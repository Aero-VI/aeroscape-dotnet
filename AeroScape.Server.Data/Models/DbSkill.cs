using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AeroScape.Server.Data.Models;

/// <summary>
/// Per-skill level and XP for a player.
/// Legacy stored as: skill{index}:{level},{xp}
/// </summary>
[Table("Skills")]
public class DbSkill
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int PlayerId { get; set; }

    /// <summary>Skill index (0=Attack, 1=Defence, 2=Strength, 3=Hitpoints, 4=Ranged, 5=Prayer, 6=Magic, etc.)</summary>
    public int SkillIndex { get; set; }

    public int Level { get; set; } = 1;
    public int Experience { get; set; }

    [ForeignKey(nameof(PlayerId))]
    public DbPlayer Player { get; set; } = null!;
}
