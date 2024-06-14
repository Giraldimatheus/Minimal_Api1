using System.ComponentModel.DataAnnotations.Schema;

namespace Minimal_Api1.Models
{
    [Table("tb_champions")]
    public class Champions
    {
        //  champion_name, roles, base_health, base_mana, base_armor, base_attack_damage, gold_efficiency

        public int Id { get; set; }

        [Column("champion_name")]
        public string ChampionName { get; set; }

        [Column("roles")]
        public string Roles { get; set; }

        [Column("base_health")]
        public string BaseHealth { get; set; }

        [Column("base_mana")]
        public string BaseMana { get; set; }

        [Column("base_armor")]
        public string BaseArmor { get; set; }

        [Column("base_attack_damage")]
        public string BaseAtackDamage { get; set; }

        [Column("gold_efficiency")]
        public string GoldEfficiency { get; set; }
    }
}
