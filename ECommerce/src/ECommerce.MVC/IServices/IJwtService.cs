namespace ECommerce.MVC.IServices
{
    /// <summary>
    /// Generates JWT tokens that are sent as Bearer tokens to the three backend APIs.
    /// The MVC app acts as the token issuer; the APIs validate against the same key.
    /// </summary>
    public interface IJwtService 
    {
        string GenerateToken(string userId, string email, string name, string role);
    }
}
