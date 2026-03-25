using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroScape.Server.Data.Models;

/// <summary>
/// Inventory item slot for a player.
/// Legacy stored as: item{slot}:{itemId},{amount}
/// </summary>
[Table("Items")]
public class DbItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int PlayerId { get; set; }

    /// <summary>Inventory slot index (0-27).</summary>
    public int Slot { get; set; }

    public int ItemId { get; set; }
    public int Amount { get; set; }

    [ForeignKey(nameof(PlayerId))]
    public DbPlayer Player { get; set; } = null!;
}
