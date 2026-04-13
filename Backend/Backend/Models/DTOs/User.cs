using Backend.Models.Views;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Backend.Models.DTOs
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(255)]
        public string? Name { get; set; }
        [Required, MaxLength(255)]
        public string? Surname { get; set; }
        [Required, MaxLength(255), EmailAddress]
        public string? EmailAddress { get; set; }
        [Required, MaxLength(255)]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public byte[]? Salt { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [DataType(DataType.DateTime)]
        public DateTime ModifiedAt { get; set; } = DateTime.Now;
        public ICollection<Collaboration>? Collaborations { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<RefreshToken>? RefreshTokens { get; set; }
        public User() { }
        public User(UserView user)
        {
            Name = user.Name;
            Surname = user.Surname;
            EmailAddress = user.EmailAddress;
            Username = user.Username;
            CreatePasswordHash(user.Password ?? string.Empty);
        }
        protected void CreatePasswordHash(string password)
        {
            // 1. Create a 128-bit salt (16 bytes)
            Salt = RandomNumberGenerator.GetBytes(128 / 8);

            // 2. Derive a 256-bit subkey (hash)
            Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
        }
        public bool VerifyPassword(string password)
        {
            // 1. Re-hash the entered password using the same salt and parameters
            string newlyHashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Salt!,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            // 2. Compare hashes (use FixedTimeEquals to prevent timing attacks)
            return CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(newlyHashed),
                Convert.FromBase64String(Password!));
        }
        public void UpdatePassword(string newPassword)
        {
            CreatePasswordHash(newPassword);
            ModifiedAt = DateTime.Now;
        }
        public void UpdateUser(string name, string surname, string username)
        {
            Name = name;
            Surname = surname;
            Username = username;
            ModifiedAt = DateTime.Now;
        }
    }
}
