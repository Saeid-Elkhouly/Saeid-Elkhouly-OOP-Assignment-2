namespace PatternsLab
{
    public class Weapon
{
    public string Name { get; set; } = null!;
    public int Damage { get; set; }

    public Weapon()
    {
    }

    public Weapon(Weapon source)
    {
        Name = source.Name;
        Damage = source.Damage;
    }
}

    public abstract class Enemy
{
    private string _modelData;

    public string Name { get; set; } = null!;
    public int Health { get; set; }
    public Weapon Weapon { get; set; } = null!;
    public List<string> Abilities { get; set; } = [];



        public string ModelId => _modelData;

        protected Enemy()
        {
            Console.WriteLine("   ...loading 3D model (slow)...");
            Thread.Sleep(500);
            _modelData = "MODEL_" + Guid.NewGuid().ToString("N")[..6];
        }

        protected Enemy(Enemy source)
        {
            _modelData = source._modelData;
            Name = source.Name;
            Health = source.Health;
            Weapon = new Weapon(source.Weapon);
            Abilities = [.. source.Abilities];
        }

        public abstract Enemy Clone();
    }

    public class Orc : Enemy
    {
        public Orc()
        {
            Name = "Orc";
            Health = 100;
            Weapon = new Weapon
            {
                Name = "Axe",
                Damage = 25
            };
            Abilities.Add("Rage");
        }

        private Orc(Orc source) : base(source)
        {
        }

        public override Orc Clone()
        {
            return new Orc(this);
        }
    }

    public class Elf : Enemy
    {
        public Elf()
        {
            Name = "Elf";
            Health = 70;
            Weapon = new Weapon
            {
                Name = "Bow",
                Damage = 18
            };
            Abilities.Add("Stealth");
        }

        private Elf(Elf source) : base(source)
        {
        }

        public override Elf Clone()
        {
            return new Elf(this);
        }
    }


    public class EnemyRegistry
    {
        private readonly Dictionary<string, Enemy> _prototypes = new();

        public void Register(string name, Enemy prototype)
        {
            _prototypes[name] = prototype;
        }

        public Enemy Create(string name)
        {
            return _prototypes[name].Clone();
        }
    }
}