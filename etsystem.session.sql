
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";


-- TABLES

-- ROLE: User role definitions
CREATE TABLE IF NOT EXISTS role (
    role_id     UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    role_name   VARCHAR(50) NOT NULL UNIQUE,
    description VARCHAR(255),
    created_at  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at  TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

COMMENT ON TABLE role IS 'System user roles';
COMMENT ON COLUMN role.role_name IS 'Role name: admin, staff, tutor, student';

-- roles
INSERT INTO role (role_name, description) VALUES
    ('admin',   'Full system access, including user management and system configuration'),
    ('staff',   'Can allocate/reallocate tutors, view all dashboards, and generate reports'),
    ('tutor',   'Can view assigned students, interact via system, and manage tutoring sessions'),
    ('student', 'Can view own dashboard, interact with assigned tutor, and access learning materials');
ON CONFLICT (role_name) DO NOTHING;

-- PERMISSION: Available system permissions
CREATE TABLE IF NOT EXISTS permission (
    permission_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    resource      VARCHAR(100) NOT NULL,
    action        VARCHAR(50) NOT NULL,
    description   VARCHAR(255),
    created_at    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UNIQUE(resource, action)
);

COMMENT ON TABLE permission IS 'System permissions for role-based access control';

-- Seed permissions
INSERT INTO permission (resource, action, description) VALUES
    -- Dashboard permissions
    ('dashboard', 'read_any', 'View any user dashboard'),
    ('dashboard', 'read_own', 'View own dashboard only'),
    
    -- Allocation permissions
    ('allocation', 'create', 'Create new tutor-student allocations'),
    ('allocation', 'update', 'Modify existing allocations'),
    ('allocation', 'delete', 'Delete allocations'),
    ('allocation', 'read', 'View allocation details'),
    
    -- Message permissions
    ('message', 'send', 'Send messages'),
    ('message', 'read', 'Read messages'),
    ('message', 'delete', 'Delete messages'),
    
    -- Meeting permissions
    ('meeting', 'create', 'Schedule meetings'),
    ('meeting', 'update', 'Modify meetings'),
    ('meeting', 'delete', 'Cancel meetings'),
    ('meeting', 'read', 'View meetings'),
    
    -- Document permissions
    ('document', 'upload', 'Upload documents'),
    ('document', 'download', 'Download documents'),
    ('document', 'delete', 'Delete documents'),
    ('document', 'read', 'View documents'),
    
    -- Blog permissions
    ('blog', 'create', 'Create blog posts'),
    ('blog', 'update', 'Edit blog posts'),
    ('blog', 'delete', 'Delete blog posts'),
    ('blog', 'publish', 'Publish/unpublish posts'),
    
    -- Report permissions
    ('report', 'generate', 'Generate system reports'),
    ('report', 'export', 'Export report data'),
    
    -- User management
    ('user', 'create', 'Create users'),
    ('user', 'update', 'Update user information'),
    ('user', 'delete', 'Delete users'),
    ('user', 'read', 'View user information');


-- ROLE_PERMISSION the juction table for role-permission mapping
CREATE TABLE IF NOT EXISTS role_permission (
    role_id       UUID NOT NULL REFERENCES role(role_id) ON DELETE CASCADE,
    permission_id UUID NOT NULL REFERENCES permission(permission_id) ON DELETE CASCADE,
    granted_at    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    granted_by    UUID,
    PRIMARY KEY (role_id, permission_id)
);


-- USER MANAGEMENT
-- USER: System users table

CREATE TABLE IF NOT EXISTS "user" (
    user_id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    username         VARCHAR(50) NOT NULL UNIQUE,
    email            VARCHAR(254) NOT NULL UNIQUE,
    password_hash    VARCHAR(255) NOT NULL,
    full_name        VARCHAR(150) NOT NULL,
    role_id          UUID NOT NULL REFERENCES role(role_id) ON DELETE RESTRICT,
    is_active        BOOLEAN NOT NULL DEFAULT TRUE,
    email_verified   BOOLEAN NOT NULL DEFAULT FALSE,
    last_login_at    TIMESTAMPTZ,
    created_at       TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at       TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    deleted_at       TIMESTAMPTZ,  -- Soft delete
    
    CONSTRAINT chk_email_format CHECK (email ~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$'),
    CONSTRAINT chk_username_format CHECK (username ~* '^[a-z0-9_]{3,50}$')
);

COMMENT ON TABLE "user" IS 'System users with role-based access';
COMMENT ON COLUMN "user".deleted_at IS 'Soft delete timestamp - user data retained for compliance';

CREATE INDEX IF NOT EXISTS idx_user_email ON "user"(email) WHERE deleted_at IS NULL;
CREATE INDEX IF NOT EXISTS idx_user_username ON "user"(username) WHERE deleted_at IS NULL;
CREATE INDEX IF NOT EXISTS idx_user_role_id ON "user"(role_id);
CREATE INDEX IF NOT EXISTS idx_user_active ON "user"(is_active) WHERE is_active = TRUE AND deleted_at IS NULL;
CREATE INDEX IF NOT EXISTS idx_user_deleted ON "user"(deleted_at) WHERE deleted_at IS NOT NULL;
CREATE INDEX IF NOT EXISTS idx_user_role_active ON "user"(role_id, is_active) WHERE deleted_at IS NULL AND is_active = TRUE;


-- USER_PROFILE: Extended user profile information
CREATE TABLE IF NOT EXISTS user_profile (
    profile_id      UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id         UUID NOT NULL REFERENCES "user"(user_id) ON DELETE CASCADE,
    phone_number    VARCHAR(20),
    date_of_birth   DATE,
    address         TEXT,
    bio             TEXT,
    avatar_url      VARCHAR(500),
    preferences     JSONB DEFAULT '{}'::jsonb,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    
    UNIQUE(user_id),
    CONSTRAINT chk_phone_format CHECK (phone_number IS NULL OR phone_number ~ '^[0-9+\-\s()]{10,20}$')
);

CREATE INDEX IF NOT EXISTS idx_user_profile_user ON user_profile(user_id);
CREATE INDEX IF NOT EXISTS idx_user_profile_preferences ON user_profile USING GIN (preferences);

-- TUTOR_STUDENT: Core allocation table with history tracking
 
CREATE TABLE IF NOT EXISTS tutor_student (
    allocation_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tutor_id      UUID NOT NULL REFERENCES "user"(user_id) ON DELETE RESTRICT,
    student_id    UUID NOT NULL REFERENCES "user"(user_id) ON DELETE RESTRICT,
    allocated_by  UUID NOT NULL REFERENCES "user"(user_id) ON DELETE RESTRICT,
    allocated_at  TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    deallocated_at TIMESTAMPTZ,
    deallocated_by UUID REFERENCES "user"(user_id) ON DELETE SET NULL,
    is_current    BOOLEAN NOT NULL DEFAULT TRUE,
    notes         TEXT,
    created_at    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    
    CONSTRAINT chk_tutor_not_student CHECK (tutor_id <> student_id),
    CONSTRAINT chk_allocation_consistency CHECK (
        (is_current = TRUE AND deallocated_at IS NULL) OR
        (is_current = FALSE AND deallocated_at IS NOT NULL)
    )
);

COMMENT ON TABLE tutor_student IS 'Tutor-student allocations with complete history';
COMMENT ON COLUMN tutor_student.is_current IS 'Indicates active allocation';
COMMENT ON COLUMN tutor_student.deallocated_at IS 'When allocation ended - only set when is_current = FALSE';

-- Unique constraint for current active allocation per student
CREATE UNIQUE INDEX idx_tutor_student_current_active 
    ON tutor_student(student_id) 
    WHERE is_current = TRUE;

CREATE INDEX IF NOT EXISTS idx_tutor_student_tutor_id ON tutor_student(tutor_id);
CREATE INDEX IF NOT EXISTS idx_tutor_student_student_id ON tutor_student(student_id);
CREATE INDEX IF NOT EXISTS idx_tutor_student_allocated_at ON tutor_student(allocated_at);
CREATE INDEX IF NOT EXISTS idx_tutor_student_deallocated_at ON tutor_student(deallocated_at) 
    WHERE deallocated_at IS NOT NULL;
CREATE INDEX IF NOT EXISTS idx_tutor_student_current_tutor ON tutor_student(tutor_id, is_current) 
    WHERE is_current = TRUE;


-- MESSAGE: Direct messages between users

CREATE TABLE IF NOT EXISTS message (
    message_id     UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    allocation_id  UUID NOT NULL REFERENCES tutor_student(allocation_id) ON DELETE CASCADE,
    sender_id      UUID NOT NULL REFERENCES "user"(user_id) ON DELETE RESTRICT,
    recipient_id   UUID NOT NULL REFERENCES "user"(user_id) ON DELETE RESTRICT,
    subject        VARCHAR(200),
    body           TEXT NOT NULL,
    is_read        BOOLEAN NOT NULL DEFAULT FALSE,
    read_at        TIMESTAMPTZ,
    parent_message_id UUID REFERENCES message(message_id) ON DELETE SET NULL,  -- For threading
    sent_at        TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at     TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    deleted_by_sender BOOLEAN NOT NULL DEFAULT FALSE,
    deleted_by_recipient BOOLEAN NOT NULL DEFAULT FALSE,
    
    CONSTRAINT chk_message_different_users CHECK (sender_id <> recipient_id),
    CONSTRAINT chk_message_body CHECK (char_length(body) > 0 AND char_length(body) <= 10000)
);

COMMENT ON TABLE message IS 'User-to-user messages within tutoring context';

CREATE INDEX IF NOT EXISTS idx_message_allocation_id ON message(allocation_id);
CREATE INDEX IF NOT EXISTS idx_message_sender ON message(sender_id, deleted_by_sender) WHERE deleted_by_sender = FALSE;
CREATE INDEX IF NOT EXISTS idx_message_recipient ON message(recipient_id, deleted_by_recipient) WHERE deleted_by_recipient = FALSE;
CREATE INDEX IF NOT EXISTS idx_message_sent_at ON message(sent_at);
CREATE INDEX IF NOT EXISTS idx_message_parent_id ON message(parent_message_id);
CREATE INDEX IF NOT EXISTS idx_message_parent_thread ON message(parent_message_id, sent_at) WHERE parent_message_id IS NOT NULL;

-- MEETING: Scheduled meetings between tutors and students

CREATE TABLE IF NOT EXISTS meeting (
    meeting_id      UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    allocation_id   UUID NOT NULL REFERENCES tutor_student(allocation_id) ON DELETE CASCADE,
    scheduled_by    UUID NOT NULL REFERENCES "user"(user_id) ON DELETE RESTRICT,
    scheduled_at    TIMESTAMPTZ NOT NULL,
    duration_minutes INTEGER NOT NULL DEFAULT 60 CHECK (duration_minutes BETWEEN 15 AND 480),
    meeting_type    VARCHAR(20) NOT NULL CHECK (meeting_type IN ('real', 'virtual')),
    location        VARCHAR(500),
    meeting_link    VARCHAR(500),
    agenda          TEXT,
    notes           TEXT,
    status          VARCHAR(20) NOT NULL DEFAULT 'scheduled' CHECK (status IN ('scheduled', 'confirmed', 'in_progress', 'completed', 'cancelled', 'no_show')),
    cancellation_reason TEXT,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    
    CONSTRAINT chk_meeting_future CHECK (scheduled_at > NOW() - INTERVAL '1 hour'),
    CONSTRAINT chk_meeting_location CHECK (
        (meeting_type = 'virtual' AND meeting_link IS NOT NULL) OR
        (meeting_type = 'real' AND location IS NOT NULL)
    )
);

COMMENT ON TABLE meeting IS 'Scheduled tutoring sessions';

CREATE INDEX IF NOT EXISTS idx_meeting_allocation_id ON meeting(allocation_id);
CREATE INDEX IF NOT EXISTS idx_meeting_allocation_scheduled ON meeting(allocation_id, scheduled_at);
CREATE INDEX IF NOT EXISTS idx_meeting_status ON meeting(status, scheduled_at) WHERE status NOT IN ('completed', 'cancelled');
CREATE INDEX IF NOT EXISTS idx_meeting_date_range ON meeting(scheduled_at) WHERE scheduled_at >= NOW();
CREATE INDEX IF NOT EXISTS idx_meeting_scheduled_by ON meeting(scheduled_by);

-- DOCUMENT: Shared documents and resources

CREATE TABLE IF NOT EXISTS document (
    document_id      UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    allocation_id    UUID NOT NULL REFERENCES tutor_student(allocation_id) ON DELETE CASCADE,
    uploaded_by      UUID NOT NULL REFERENCES "user"(user_id) ON DELETE RESTRICT,
    filename         VARCHAR(255) NOT NULL,
    original_name    VARCHAR(255) NOT NULL,
    file_path        VARCHAR(500) NOT NULL,
    file_size_bytes  BIGINT NOT NULL CHECK (file_size_bytes > 0),
    mime_type        VARCHAR(100) NOT NULL,
    description      TEXT,
    version          INTEGER NOT NULL DEFAULT 1,
    parent_document_id UUID REFERENCES document(document_id) ON DELETE SET NULL,
    is_deleted       BOOLEAN NOT NULL DEFAULT FALSE,
    uploaded_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at       TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    
    CONSTRAINT chk_file_size CHECK (file_size_bytes <= 104857600) -- 100MB limit
);

COMMENT ON TABLE document IS 'Shared documents and resources for tutoring sessions';

CREATE INDEX IF NOT EXISTS idx_document_allocation_id ON document(allocation_id);
CREATE INDEX IF NOT EXISTS idx_document_allocation_active ON document(allocation_id, is_deleted) WHERE is_deleted = FALSE;
CREATE INDEX IF NOT EXISTS idx_document_uploaded_by ON document(uploaded_by);
CREATE INDEX IF NOT EXISTS idx_document_parent_id ON document(parent_document_id);
CREATE INDEX IF NOT EXISTS idx_document_parent_version ON document(parent_document_id, version) WHERE parent_document_id IS NOT NULL;


-- DOCUMENT_COMMENT: Comments on documents

CREATE TABLE IF NOT EXISTS document_comment (
    comment_id   UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    document_id  UUID NOT NULL REFERENCES document(document_id) ON DELETE CASCADE,
    author_id    UUID NOT NULL REFERENCES "user"(user_id) ON DELETE RESTRICT,
    body         TEXT NOT NULL CHECK (char_length(body) > 0 AND char_length(body) <= 2000),
    created_at   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at   TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_doc_comment_document_id ON document_comment(document_id);
CREATE INDEX IF NOT EXISTS idx_doc_comment_author_id ON document_comment(author_id);


-- BLOG_POST: Knowledge base and announcements

CREATE TABLE IF NOT EXISTS blog_post (
    post_id       UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    author_id     UUID NOT NULL REFERENCES "user"(user_id) ON DELETE RESTRICT,
    title         VARCHAR(300) NOT NULL,
    slug          VARCHAR(300) NOT NULL UNIQUE,
    excerpt       VARCHAR(500),
    body          TEXT NOT NULL,
    featured_image VARCHAR(500),
    tags          TEXT[] DEFAULT '{}',
    visibility    VARCHAR(20) NOT NULL DEFAULT 'private' CHECK (visibility IN ('private', 'tutor_only', 'staff_only', 'public')),
    status        VARCHAR(20) NOT NULL DEFAULT 'draft' CHECK (status IN ('draft', 'published', 'archived')),
    published_at  TIMESTAMPTZ,
    created_at    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    deleted_at    TIMESTAMPTZ
);

COMMENT ON TABLE blog_post IS 'Knowledge base articles and announcements';

CREATE INDEX IF NOT EXISTS idx_blog_post_author_id ON blog_post(author_id);
CREATE INDEX IF NOT EXISTS idx_blog_post_status ON blog_post(status, visibility, published_at) 
    WHERE status = 'published' AND deleted_at IS NULL;
CREATE INDEX IF NOT EXISTS idx_blog_post_tags ON blog_post USING GIN(tags);
CREATE INDEX IF NOT EXISTS idx_blog_post_slug ON blog_post(slug);


ALTER TABLE blog_post ADD COLUMN IF NOT EXISTS search_vector tsvector
    GENERATED ALWAYS AS (
        setweight(to_tsvector('english', COALESCE(title, '')), 'A') ||
        setweight(to_tsvector('english', COALESCE(excerpt, '')), 'B') ||
        setweight(to_tsvector('english', COALESCE(body, '')), 'C')
    ) STORED;


CREATE INDEX IF NOT EXISTS idx_blog_post_search ON blog_post USING GIN(search_vector) 
    WHERE status = 'published' AND deleted_at IS NULL;


-- BLOG_COMMENT: Comments on blog posts

CREATE TABLE IF NOT EXISTS blog_comment (
    comment_id   UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    post_id      UUID NOT NULL REFERENCES blog_post(post_id) ON DELETE CASCADE,
    author_id    UUID NOT NULL REFERENCES "user"(user_id) ON DELETE RESTRICT,
    parent_comment_id UUID REFERENCES blog_comment(comment_id) ON DELETE SET NULL,
    body         TEXT NOT NULL CHECK (char_length(body) > 0 AND char_length(body) <= 2000),
    is_approved  BOOLEAN NOT NULL DEFAULT TRUE,
    created_at   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at   TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS idx_blog_comment_post ON blog_comment(post_id, is_approved);
CREATE INDEX IF NOT EXISTS idx_blog_comment_parent ON blog_comment(parent_comment_id);


-- NOTIFICATION: User notifications

CREATE TABLE IF NOT EXISTS notification (
    notification_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id         UUID NOT NULL REFERENCES "user"(user_id) ON DELETE CASCADE,
    type            VARCHAR(50) NOT NULL,
    title           VARCHAR(200) NOT NULL,
    message         TEXT NOT NULL,
    data            JSONB,
    related_entity  VARCHAR(50),
    related_id      UUID,
    is_read         BOOLEAN NOT NULL DEFAULT FALSE,
    read_at         TIMESTAMPTZ,
    is_emailed      BOOLEAN NOT NULL DEFAULT FALSE,
    emailed_at      TIMESTAMPTZ,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    expires_at      TIMESTAMPTZ
);

COMMENT ON TABLE notification IS 'User notifications for system events';

-- Notification indexes
CREATE INDEX IF NOT EXISTS idx_notification_user ON notification(user_id, is_read, created_at) WHERE is_read = FALSE;
CREATE INDEX IF NOT EXISTS idx_notification_type ON notification(type, created_at);
CREATE INDEX IF NOT EXISTS idx_notification_expires ON notification(expires_at) WHERE expires_at IS NOT NULL;
CREATE INDEX IF NOT EXISTS idx_notification_user_unread ON notification(user_id, is_read) WHERE is_read = FALSE;


-- AUDIT_LOG: System audit trail

CREATE TABLE IF NOT EXISTS audit_log (
    audit_id     UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    table_name   VARCHAR(50) NOT NULL,
    record_id    UUID NOT NULL,
    action       VARCHAR(10) NOT NULL CHECK (action IN ('INSERT', 'UPDATE', 'DELETE', 'SOFT_DELETE', 'RESTORE')),
    old_data     JSONB,
    new_data     JSONB,
    changed_by   UUID REFERENCES "user"(user_id),
    ip_address   INET,
    user_agent   TEXT,
    changed_at   TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

COMMENT ON TABLE audit_log IS 'Comprehensive audit trail for compliance';

CREATE INDEX IF NOT EXISTS idx_audit_table_record ON audit_log(table_name, record_id);
CREATE INDEX IF NOT EXISTS idx_audit_changed_by ON audit_log(changed_by, changed_at);
CREATE INDEX IF NOT EXISTS idx_audit_changed_at ON audit_log(changed_at);




CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER LANGUAGE plpgsql AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$;

-- Apply update triggers to all tables with updated_at
DROP TRIGGER IF EXISTS update_user_updated_at ON "user";
CREATE TRIGGER update_user_updated_at BEFORE UPDATE ON "user" FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

DROP TRIGGER IF EXISTS update_user_profile_updated_at ON user_profile;
CREATE TRIGGER update_user_profile_updated_at BEFORE UPDATE ON user_profile FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

DROP TRIGGER IF EXISTS update_tutor_student_updated_at ON tutor_student;
CREATE TRIGGER update_tutor_student_updated_at BEFORE UPDATE ON tutor_student FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

DROP TRIGGER IF EXISTS update_message_updated_at ON message;
CREATE TRIGGER update_message_updated_at BEFORE UPDATE ON message FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

DROP TRIGGER IF EXISTS update_meeting_updated_at ON meeting;
CREATE TRIGGER update_meeting_updated_at BEFORE UPDATE ON meeting FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

DROP TRIGGER IF EXISTS update_document_updated_at ON document;
CREATE TRIGGER update_document_updated_at BEFORE UPDATE ON document FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

DROP TRIGGER IF EXISTS update_blog_post_updated_at ON blog_post;
CREATE TRIGGER update_blog_post_updated_at BEFORE UPDATE ON blog_post FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

--  Auto-deactivate old allocations trigger

CREATE OR REPLACE FUNCTION handle_allocation_deactivation()
RETURNS TRIGGER LANGUAGE plpgsql AS $$
BEGIN
    IF NEW.is_current = TRUE AND OLD.is_current = FALSE THEN
        -- Reactivation - clear deallocation info
        NEW.deallocated_at := NULL;
        NEW.deallocated_by := NULL;
    ELSIF NEW.is_current = FALSE AND OLD.is_current = TRUE THEN
        -- Deactivation - set deallocation timestamp
        IF NEW.deallocated_at IS NULL THEN
            NEW.deallocated_at := NOW();
        END IF;
    END IF;
    RETURN NEW;
END;
$$;

DROP TRIGGER IF EXISTS handle_allocation_deactivation ON tutor_student;
CREATE TRIGGER handle_allocation_deactivation
    BEFORE UPDATE ON tutor_student
    FOR EACH ROW
    EXECUTE FUNCTION handle_allocation_deactivation();


-- Auto-notification for meeting creation

CREATE OR REPLACE FUNCTION notify_meeting_created()
RETURNS TRIGGER LANGUAGE plpgsql AS $$
DECLARE
    tutor_id UUID;
    student_id UUID;
BEGIN
    -- Get tutor and student from allocation
    SELECT tutor_id, student_id INTO tutor_id, student_id
    FROM tutor_student
    WHERE allocation_id = NEW.allocation_id;
    
    -- Create notifications for both parties
    INSERT INTO notification (user_id, type, title, message, related_entity, related_id)
    VALUES 
        (tutor_id, 'meeting_scheduled', 'Meeting Scheduled', 
         format('A meeting has been scheduled for %s', to_char(NEW.scheduled_at, 'YYYY-MM-DD HH24:MI')), 
         'meeting', NEW.meeting_id),
        (student_id, 'meeting_scheduled', 'Meeting Scheduled', 
         format('A meeting has been scheduled for %s', to_char(NEW.scheduled_at, 'YYYY-MM-DD HH24:MI')), 
         'meeting', NEW.meeting_id);
    
    RETURN NEW;
END;
$$;

DROP TRIGGER IF EXISTS notify_meeting_created ON meeting;
CREATE TRIGGER notify_meeting_created
    AFTER INSERT ON meeting
    FOR EACH ROW
    EXECUTE FUNCTION notify_meeting_created();

-- Meeting conflict check

CREATE OR REPLACE FUNCTION check_meeting_conflict()
RETURNS TRIGGER LANGUAGE plpgsql AS $$
DECLARE
    tutor_id UUID;
    student_id UUID;
    conflict_exists BOOLEAN;
BEGIN
    -- Get tutor and student from allocation
    SELECT tutor_id, student_id INTO tutor_id, student_id
    FROM tutor_student
    WHERE allocation_id = NEW.allocation_id;
    
    -- Check for overlapping meetings
    SELECT EXISTS (
        SELECT 1 FROM meeting m
        JOIN tutor_student ts ON ts.allocation_id = m.allocation_id
        WHERE (ts.tutor_id = tutor_id OR ts.student_id = student_id)
            AND m.status NOT IN ('cancelled', 'completed')
            AND m.meeting_id != COALESCE(NEW.meeting_id, '00000000-0000-0000-0000-000000000000'::UUID)
            AND (
                (m.scheduled_at <= NEW.scheduled_at + (NEW.duration_minutes || ' minutes')::INTERVAL
                 AND m.scheduled_at + (m.duration_minutes || ' minutes')::INTERVAL >= NEW.scheduled_at)
            )
    ) INTO conflict_exists;
    
    IF conflict_exists THEN
        RAISE EXCEPTION 'Meeting time conflicts with existing meeting for tutor or student';
    END IF;
    
    RETURN NEW;
END;
$$;

DROP TRIGGER IF EXISTS check_meeting_conflict ON meeting;
CREATE TRIGGER check_meeting_conflict
    BEFORE INSERT OR UPDATE ON meeting
    FOR EACH ROW
    EXECUTE FUNCTION check_meeting_conflict();


-- 9. ANALYTICS & REPORTING VIEWS


-- Active students without tutor
CREATE OR REPLACE VIEW active_students_without_tutor AS
SELECT 
    u.user_id,
    u.full_name,
    u.email,
    u.created_at
FROM "user" u
WHERE u.role_id = (SELECT role_id FROM role WHERE role_name = 'student')
    AND u.is_active = TRUE
    AND u.deleted_at IS NULL
    AND NOT EXISTS (
        SELECT 1 FROM tutor_student ts
        WHERE ts.student_id = u.user_id AND ts.is_current = TRUE
    );

-- Tutor performance metrics

CREATE OR REPLACE VIEW tutor_performance_metrics AS
SELECT 
    u.user_id,
    u.full_name,
    COUNT(DISTINCT ts.student_id) FILTER (WHERE ts.is_current = TRUE) AS active_students,
    COUNT(DISTINCT ts.student_id) AS total_students_allocated,
    COUNT(DISTINCT m.message_id) AS total_messages,
    COUNT(DISTINCT mt.meeting_id) AS total_meetings,
    COUNT(DISTINCT mt.meeting_id) FILTER (WHERE mt.status = 'completed') AS completed_meetings,
    ROUND(COUNT(m.message_id)::NUMERIC / NULLIF(COUNT(DISTINCT ts.student_id), 0), 2) AS avg_messages_per_student,
    ROUND(COUNT(mt.meeting_id)::NUMERIC / NULLIF(COUNT(DISTINCT ts.student_id), 0), 2) AS avg_meetings_per_student,
    MAX(mt.scheduled_at) FILTER (WHERE mt.status = 'completed') AS last_meeting
FROM "user" u
LEFT JOIN tutor_student ts ON ts.tutor_id = u.user_id
LEFT JOIN message m ON m.allocation_id = ts.allocation_id
LEFT JOIN meeting mt ON mt.allocation_id = ts.allocation_id
WHERE u.role_id = (SELECT role_id FROM role WHERE role_name = 'tutor')
    AND u.deleted_at IS NULL
GROUP BY u.user_id, u.full_name;


-- Student engagement dashboard
CREATE OR REPLACE VIEW student_engagement_dashboard AS
SELECT 
    u.user_id,
    u.full_name,
    u.email,
    ts.tutor_id,
    tutor.full_name AS tutor_name,
    COUNT(DISTINCT m.message_id) AS message_count,
    MAX(m.sent_at) AS last_message,
    COUNT(DISTINCT mt.meeting_id) AS meeting_count,
    MAX(mt.scheduled_at) AS last_meeting,
    COUNT(DISTINCT d.document_id) AS document_count,
    MAX(d.uploaded_at) AS last_document,
    GREATEST(
        COALESCE(MAX(m.sent_at), '1970-01-01'::timestamp),
        COALESCE(MAX(mt.scheduled_at), '1970-01-01'::timestamp),
        COALESCE(MAX(d.uploaded_at), '1970-01-01'::timestamp)
    ) AS last_interaction,
    EXTRACT(DAY FROM NOW() - GREATEST(
        COALESCE(MAX(m.sent_at), '1970-01-01'::timestamp),
        COALESCE(MAX(mt.scheduled_at), '1970-01-01'::timestamp),
        COALESCE(MAX(d.uploaded_at), '1970-01-01'::timestamp)
    )) AS days_since_interaction
FROM "user" u
LEFT JOIN tutor_student ts ON ts.student_id = u.user_id AND ts.is_current = TRUE
LEFT JOIN "user" tutor ON tutor.user_id = ts.tutor_id
LEFT JOIN message m ON m.allocation_id = ts.allocation_id
LEFT JOIN meeting mt ON mt.allocation_id = ts.allocation_id
LEFT JOIN document d ON d.allocation_id = ts.allocation_id
WHERE u.role_id = (SELECT role_id FROM role WHERE role_name = 'student')
    AND u.is_active = TRUE
    AND u.deleted_at IS NULL
GROUP BY u.user_id, u.full_name, u.email, ts.tutor_id, tutor.full_name;


-- Weekly activity summary (fixed version)

CREATE OR REPLACE VIEW weekly_activity_summary AS
WITH 
user_weekly AS (
    SELECT 
        DATE_TRUNC('week', created_at) AS week_start,
        COUNT(DISTINCT user_id) FILTER (WHERE role_id = (SELECT role_id FROM role WHERE role_name = 'student')) AS new_students,
        COUNT(DISTINCT user_id) FILTER (WHERE role_id = (SELECT role_id FROM role WHERE role_name = 'tutor')) AS new_tutors
    FROM "user"
    WHERE created_at >= NOW() - INTERVAL '90 days'
    GROUP BY DATE_TRUNC('week', created_at)
),
message_weekly AS (
    SELECT 
        DATE_TRUNC('week', sent_at) AS week_start,
        COUNT(*) AS total_messages
    FROM message
    WHERE sent_at >= NOW() - INTERVAL '90 days'
    GROUP BY DATE_TRUNC('week', sent_at)
),
meeting_weekly AS (
    SELECT 
        DATE_TRUNC('week', created_at) AS week_start,
        COUNT(*) AS total_meetings
    FROM meeting
    WHERE created_at >= NOW() - INTERVAL '90 days'
    GROUP BY DATE_TRUNC('week', created_at)
),
document_weekly AS (
    SELECT 
        DATE_TRUNC('week', uploaded_at) AS week_start,
        COUNT(*) AS total_documents
    FROM document
    WHERE uploaded_at >= NOW() - INTERVAL '90 days' AND is_deleted = FALSE
    GROUP BY DATE_TRUNC('week', uploaded_at)
),
blog_weekly AS (
    SELECT 
        DATE_TRUNC('week', published_at) AS week_start,
        COUNT(*) AS blog_posts_published
    FROM blog_post
    WHERE published_at >= NOW() - INTERVAL '90 days' AND status = 'published'
    GROUP BY DATE_TRUNC('week', published_at)
)
SELECT 
    COALESCE(u.week_start, m.week_start, mt.week_start, d.week_start, b.week_start) AS week_start,
    COALESCE(u.new_students, 0) AS new_students,
    COALESCE(u.new_tutors, 0) AS new_tutors,
    COALESCE(m.total_messages, 0) AS total_messages,
    COALESCE(mt.total_meetings, 0) AS total_meetings,
    COALESCE(d.total_documents, 0) AS total_documents,
    COALESCE(b.blog_posts_published, 0) AS blog_posts_published
FROM user_weekly u
FULL OUTER JOIN message_weekly m ON u.week_start = m.week_start
FULL OUTER JOIN meeting_weekly mt ON u.week_start = mt.week_start
FULL OUTER JOIN document_weekly d ON u.week_start = d.week_start
FULL OUTER JOIN blog_weekly b ON u.week_start = b.week_start
WHERE COALESCE(u.week_start, m.week_start, mt.week_start, d.week_start, b.week_start) IS NOT NULL
ORDER BY week_start DESC;


-- Students needing attention
CREATE OR REPLACE VIEW students_needing_attention AS
SELECT 
    student_id,
    full_name,
    email,
    tutor_id,
    tutor_name,
    last_interaction,
    days_since_interaction,
    CASE 
        WHEN days_since_interaction >= 28 THEN 'critical'
        WHEN days_since_interaction >= 14 THEN 'warning'
        WHEN days_since_interaction >= 7 THEN 'attention'
        ELSE 'good'
    END AS attention_level
FROM student_engagement_dashboard
WHERE days_since_interaction >= 7
    AND last_interaction > '1970-01-01'::timestamp
ORDER BY days_since_interaction DESC;


-- INITIAL DATA SETUP (Permissions)

-- Grant admin all permissions
INSERT INTO role_permission (role_id, permission_id, granted_by)
SELECT 
    r.role_id,
    p.permission_id,
    NULL
FROM role r
CROSS JOIN permission p
WHERE r.role_name = 'admin'
ON CONFLICT (role_id, permission_id) DO NOTHING;

-- Grant staff permissions
INSERT INTO role_permission (role_id, permission_id, granted_by)
SELECT 
    r.role_id,
    p.permission_id,
    NULL
FROM role r
CROSS JOIN permission p
WHERE r.role_name = 'staff'
    AND p.resource IN ('dashboard', 'allocation', 'report', 'user')
    AND p.action IN ('read_any', 'create', 'update', 'generate', 'export', 'read')
ON CONFLICT (role_id, permission_id) DO NOTHING;

-- Grant staff permissions
INSERT INTO role_permission (role_id, permission_id, granted_by)
SELECT 
    r.role_id,
    p.permission_id,
    NULL
FROM role r
CROSS JOIN permission p
WHERE r.role_name = 'staff'
    AND p.resource IN ('dashboard', 'allocation', 'report', 'user')
    AND p.action IN ('read_any', 'create', 'update', 'generate', 'export', 'read')
ON CONFLICT (role_id, permission_id) DO NOTHING;

-- Grant tutor permissions
INSERT INTO role_permission (role_id, permission_id, granted_by)
SELECT 
    r.role_id,
    p.permission_id,
    NULL
FROM role r
CROSS JOIN permission p
WHERE r.role_name = 'tutor'
    AND (
        (p.resource = 'dashboard' AND p.action = 'read_own') OR
        (p.resource = 'message' AND p.action IN ('send', 'read')) OR
        (p.resource = 'meeting' AND p.action IN ('create', 'update', 'read')) OR
        (p.resource = 'document' AND p.action IN ('upload', 'download', 'read')) OR
        (p.resource = 'blog' AND p.action IN ('create', 'update', 'read'))
    )
ON CONFLICT (role_id, permission_id) DO NOTHING;

-- Grant student permissions
INSERT INTO role_permission (role_id, permission_id, granted_by)
SELECT 
    r.role_id,
    p.permission_id,
    NULL
FROM role r
CROSS JOIN permission p
WHERE r.role_name = 'student'
    AND (
        (p.resource = 'dashboard' AND p.action = 'read_own') OR
        (p.resource = 'message' AND p.action IN ('send', 'read')) OR
        (p.resource = 'meeting' AND p.action = 'read') OR
        (p.resource = 'document' AND p.action IN ('download', 'read')) OR
        (p.resource = 'blog' AND p.action = 'read')
    )
ON CONFLICT (role_id, permission_id) DO NOTHING;


-- MAINTENANCE FUNCTIONS
-- Clean old notifications

CREATE OR REPLACE FUNCTION cleanup_old_notifications()
RETURNS void LANGUAGE plpgsql AS $$
BEGIN
    DELETE FROM notification
    WHERE is_read = TRUE
        AND created_at < NOW() - INTERVAL '90 days';
    
    RAISE NOTICE 'Cleaned up old notifications';
END;
$$;


-- Archive old audit logs
CREATE OR REPLACE FUNCTION archive_old_audit_logs(archive_days INTEGER DEFAULT 365)
RETURNS void LANGUAGE plpgsql AS $$
BEGIN
    -- Create archive table if not exists
    CREATE TABLE IF NOT EXISTS audit_log_archive (LIKE audit_log INCLUDING ALL);
    
    -- Move logs older than archive_days
    WITH moved AS (
        DELETE FROM audit_log
        WHERE changed_at < NOW() - (archive_days || ' days')::INTERVAL
        RETURNING *
    )
    INSERT INTO audit_log_archive SELECT * FROM moved;
    
    RAISE NOTICE 'Archived old audit logs';
END;
$$;


-- Search blog posts function
CREATE OR REPLACE FUNCTION search_blog_posts(search_query TEXT, visibility_filter TEXT DEFAULT NULL)
RETURNS TABLE (
    post_id UUID,
    title VARCHAR(300),
    excerpt VARCHAR(500),
    relevance REAL
) LANGUAGE plpgsql AS $$
BEGIN
    RETURN QUERY
    SELECT 
        bp.post_id,
        bp.title,
        bp.excerpt,
        ts_rank(bp.search_vector, websearch_to_tsquery('english', search_query)) AS relevance
    FROM blog_post bp
    WHERE bp.status = 'published'
        AND bp.deleted_at IS NULL
        AND (visibility_filter IS NULL OR bp.visibility = visibility_filter)
        AND bp.search_vector @@ websearch_to_tsquery('english', search_query)
    ORDER BY relevance DESC
    LIMIT 50;
END;
$$;


-- Get dashboard summary function
CREATE OR REPLACE FUNCTION get_dashboard_summary()
RETURNS TABLE (
    total_students BIGINT,
    total_tutors BIGINT,
    active_allocations BIGINT,
    unread_messages BIGINT,
    upcoming_meetings BIGINT
) LANGUAGE sql STABLE AS $$
    SELECT 
        (SELECT COUNT(*) FROM "user" WHERE role_id = (SELECT role_id FROM role WHERE role_name = 'student') AND deleted_at IS NULL) AS total_students,
        (SELECT COUNT(*) FROM "user" WHERE role_id = (SELECT role_id FROM role WHERE role_name = 'tutor') AND deleted_at IS NULL) AS total_tutors,
        (SELECT COUNT(*) FROM tutor_student WHERE is_current = TRUE) AS active_allocations,
        (SELECT COUNT(*) FROM message WHERE is_read = FALSE) AS unread_messages,
        (SELECT COUNT(*) FROM meeting WHERE scheduled_at > NOW() AND status = 'scheduled') AS upcoming_meetings;
$$;


-- Schema integrity verification
CREATE OR REPLACE FUNCTION verify_schema_integrity()
RETURNS TABLE(
    check_name TEXT,
    status TEXT,
    details TEXT
) LANGUAGE plpgsql AS $$
BEGIN
    -- Check tables exist
    RETURN QUERY
    SELECT 'Tables exist'::TEXT, 
           CASE WHEN COUNT(*) = 14 THEN 'PASS' ELSE 'FAIL' END,
           COUNT(*)::TEXT || ' of 14 tables created'
    FROM information_schema.tables 
    WHERE table_schema = 'public' 
        AND table_name IN ('role', 'permission', 'role_permission', 'user', 'user_profile', 
                          'tutor_student', 'message', 'meeting', 'document', 'document_comment',
                          'blog_post', 'blog_comment', 'notification', 'audit_log');
    
    -- Check indexes exist
    RETURN QUERY
    SELECT 'Indexes exist'::TEXT,
           CASE WHEN COUNT(*) >= 25 THEN 'PASS' ELSE 'FAIL' END,
           COUNT(*)::TEXT || ' indexes created'
    FROM pg_indexes 
    WHERE schemaname = 'public';
    
    -- Check views exist
    RETURN QUERY
    SELECT 'Views exist'::TEXT,
           CASE WHEN COUNT(*) = 5 THEN 'PASS' ELSE 'FAIL' END,
           COUNT(*)::TEXT || ' of 5 views created'
    FROM information_schema.views 
    WHERE table_schema = 'public' 
        AND table_name IN ('active_students_without_tutor', 'tutor_performance_metrics', 
                          'student_engagement_dashboard', 'weekly_activity_summary', 
                          'students_needing_attention');
    
    -- Check triggers exist
    RETURN QUERY
    SELECT 'Triggers exist'::TEXT,
           CASE WHEN COUNT(*) >= 9 THEN 'PASS' ELSE 'FAIL' END,
           COUNT(*)::TEXT || ' triggers created'
    FROM information_schema.triggers 
    WHERE trigger_schema = 'public';
END;
$$;

