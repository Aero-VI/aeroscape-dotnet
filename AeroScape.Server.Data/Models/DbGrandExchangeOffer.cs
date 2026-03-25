using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroScape.Server.Data.Models;

/// <summary>
/// Grand Exchange offer for a player.
/// Legacy stored as: offerItem{i}, offerAmount{i}, currentAmount{i}, offerType{i}, offerPrice{i}
/// </summary>
[Table("GrandExchangeOffers")]
public class DbGrandExchangeOffer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int PlayerId { get; set; }

    /// <summary>GE slot index (0-5).</summary>
    public int Slot { get; set; }

    public int ItemId { get; set; }
    public int OfferAmount { get; set; }
    public int CurrentAmount { get; set; }

    /// <summary>0 = inactive, 1 = buy, 2 = sell.</summary>
    public int OfferType { get; set; }

    public int Price { get; set; }

    [ForeignKey(nameof(PlayerId))]
    public DbPlayer Player { get; set; } = null!;
}
