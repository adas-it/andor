namespace Andor.ComponentTests.Common;

public static class HttpClientTestExtensions
{
    /// <summary>
    /// Impersonates <paramref name="user"/> on every subsequent request made with this client.
    /// </summary>
    public static void SetTestUser(this HttpClient client, TestUser user)
    {
        client.DefaultRequestHeaders.Remove(TestAuthHeaders.Anonymous);
        client.DefaultRequestHeaders.Remove(TestAuthHeaders.UserId);
        client.DefaultRequestHeaders.Remove(TestAuthHeaders.Group);
        client.DefaultRequestHeaders.Remove(TestAuthHeaders.Tenant);

        client.DefaultRequestHeaders.Add(TestAuthHeaders.UserId, user.Id.ToString());
        client.DefaultRequestHeaders.Add(TestAuthHeaders.Group, user.Group);
        client.DefaultRequestHeaders.Add(TestAuthHeaders.Tenant, user.Tenant);
    }

    /// <summary>
    /// Makes every subsequent request with this client look like it carries no credentials at
    /// all, so <c>[Authorize]</c> endpoints reject it with 401.
    /// </summary>
    public static void SetAnonymous(this HttpClient client)
    {
        client.DefaultRequestHeaders.Remove(TestAuthHeaders.UserId);
        client.DefaultRequestHeaders.Remove(TestAuthHeaders.Group);
        client.DefaultRequestHeaders.Remove(TestAuthHeaders.Tenant);
        client.DefaultRequestHeaders.Remove(TestAuthHeaders.Anonymous);

        client.DefaultRequestHeaders.Add(TestAuthHeaders.Anonymous, "true");
    }
}
