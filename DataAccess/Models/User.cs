using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models;

public partial class User
{
    public int UserId { get; set; }
    [StringLength(100)]
    public string FullName { get; set; } = null!;
    [EmailAddress]
    public string Email { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public string? Role { get; set; }
    [StringLength (255)]
    public string? Address { get; set; }
    [Phone]
    public string? PhoneNumber { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
