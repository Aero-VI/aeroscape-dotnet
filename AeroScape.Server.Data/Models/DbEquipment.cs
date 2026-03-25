using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroScape.Server.Data.Models;

/// <summary>
/// Equipped item slot for a player.
/// Legacy stored as: equipment{slot}:{itemId},{amount}
/// </summary>
[Table("Equipment")]
public class DbEquipment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int PlayerId { get; set; }

    /// <summary>Equipment slot index (0=Head, 1=Cape, 2=Amulet, etc.).</summary>
    public int Slot { get; set; }

    public int ItemId { get; set; }
    public int Amount { get; set; }

    [ForeignKey(nameof(PlayerId))]
    public DbPlayer Player { get; set; } = null!;
}
