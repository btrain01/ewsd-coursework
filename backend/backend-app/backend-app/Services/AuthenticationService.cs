namespace smartstock_inventory_service.Services
{
    public partial class AuthenticationService
    {

        //all logic should live in the service and only be called from the controller
        public async Task<string> GetName()
        {
            return await Task.Run(() => "luyando");
        }
    }
}
