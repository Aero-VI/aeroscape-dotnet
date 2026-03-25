using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroScape.Server.Data.Models;

/// <summary>
/// Friends list entry for a player.
/// Legacy stored as: friend{index}:{encodedName}
/// </summary>
[Table("Friends")]
public class DbFriend
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int PlayerId { get; set; }

    /// <summary>The friend's username (decoded from the legacy long-encoded name).</summary>
    [Required, MaxLength(12)]
    public string FriendName { get; set; } = string.Empty;

    [ForeignKey(nameof(PlayerId))]
    public DbPlayer Player { get; set; } = null!;
}
