using System.ComponentModel.DataAnnotations.Schema;

namespace Minimal_Api1.ViewModel
{
    public class ChampionViewModel
    {
        public string ChampionName { get; set; }
        public string Roles { get; set; }
        public string BaseHealth { get; set; }
        public string BaseMana { get; set; }
        public string BaseArmor { get; set; }
        public string BaseAtackDamage { get; set; }
        public string GoldEfficiency { get; set; }
    }
}
