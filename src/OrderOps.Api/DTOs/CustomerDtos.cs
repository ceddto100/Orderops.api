namespace OrderOps.Api.DTOs;

public record CreateCustomerRequest(string Name, string Email);

public record CustomerResponse(int Id, string Name, string Email, DateTime CreatedAt);
