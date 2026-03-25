using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroScape.Server.Data.Models;

/// <summary>
/// Bank item slot for a player.
/// Legacy stored as: bankitem{slot}:{itemId},{amount}
/// </summary>
[Table("BankItems")]
public class DbBankItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int PlayerId { get; set; }

    /// <summary>Bank slot index.</summary>
    public int Slot { get; set; }

    public int ItemId { get; set; }
    public int Amount { get; set; }

    [ForeignKey(nameof(PlayerId))]
    public DbPlayer Player { get; set; } = null!;
}
