namespace Tomacco.Source.Entities
{
    public interface IBuildingKind
    {
        string Name { get; set; }
    }

    public class BuildingKind : IBuildingKind
    {
        public string Name { get; set; }
    }

    public interface IBuildingMineKind : IBuildingKind;

    public class BuildingMineKind : BuildingKind, IBuildingMineKind
    {
        public new string Name => "Mine";
    }

    public interface IBuildingChurchKind : IBuildingKind;

    public class BuildingChurchKind : BuildingKind, IBuildingChurchKind
    {
        public new string Name => "Church";
    }

    public interface IBuildingInnKind : IBuildingKind;

    public class BuildingInnKind : BuildingKind, IBuildingInnKind
    {
        public new string Name => "Inn";
    }

    public interface IBuildingMarketKind : IBuildingKind;

    public class BuildingMarketKind : BuildingKind, IBuildingMarketKind
    {
        public new string Name => "Market";
    }

    public interface IBuildingArenaKind : IBuildingKind;

    public class BuildingArenaKind : BuildingKind, IBuildingArenaKind
    {
        public new string Name => "Arena";
    }

    public interface IBuildingLibraryKind : IBuildingKind;

    public class BuildingLibraryKind : BuildingKind, IBuildingLibraryKind
    {
        public new string Name => "Library";
    }

    public interface IBuildingColosseumKind : IBuildingKind;

    public class BuildingColosseumKind : BuildingKind, IBuildingColosseumKind
    {
        public new string Name => "Colosseum";
    }

    public interface IBuildingMonumentKind : IBuildingKind;

    public class BuildingMonumentKind : BuildingKind, IBuildingMonumentKind
    {
        public new string Name => "Monument";
    }

    public interface IBuildingTownHallKind : IBuildingKind;

    public class BuildingTownHallKind : BuildingKind, IBuildingTownHallKind
    {
        public new string Name => "Town Hall";
    }
}
