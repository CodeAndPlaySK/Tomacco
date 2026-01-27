using Tomacco.Source.Entities;

namespace Domain.Entities.Factories
{
    public interface IBuildingTemplateFactory
    {
        IBuildingTemplate CreateBuildingTemplate(IBuildingKind kind, int id);
    }
    public class BuildingTemplateFactory : IBuildingTemplateFactory
    {
        private readonly IMineBuildingTemplateFactory _mineBuildingFactory;
        private readonly IChurchBuildingTemplateFactory _churchBuildingFactory;
        private readonly IInnBuildingTemplateFactory _innBuildingFactory;


        public BuildingTemplateFactory(IMineBuildingTemplateFactory mineBuildingFactory, IChurchBuildingTemplateFactory churchBuildingFactory, IInnBuildingTemplateFactory innBuildingFactory)
        {
            _mineBuildingFactory = mineBuildingFactory;
            _churchBuildingFactory = churchBuildingFactory;
            _innBuildingFactory = innBuildingFactory;
        }

        public IBuildingTemplate CreateBuildingTemplate(IBuildingKind kind, int id)
        {
            if (kind.GetType().IsAssignableTo(typeof(IBuildingMineKind)))
            {
                return _mineBuildingFactory.CreateBuildingTemplate(id);
            }
            if (kind.GetType().IsAssignableTo(typeof(IBuildingChurchKind)))
            {
                return _churchBuildingFactory.CreateBuildingTemplate(id);
            }
            if (kind.GetType().IsAssignableTo(typeof(IBuildingInnKind)))
            {
                return _innBuildingFactory.CreateBuildingTemplate(id);
            }

            throw new NotSupportedException($"Building kind '{kind.Name}' unsupported");
        }
    }

    public interface ICommonBuildingTemplateFactory
    {
        IBuildingKind Kind { get; init; }

        IBuildingTemplate CreateBuildingTemplate(int id);
    }
    public abstract class CommonBuildingTemplateFactory : ICommonBuildingTemplateFactory
    {
        public IBuildingKind Kind { get; init; }
        public CommonBuildingTemplateFactory(IBuildingKind kind)
        {
            Kind = kind;
        }
        public abstract IBuildingTemplate CreateBuildingTemplate(int id);
    }

    public interface IMineBuildingTemplateFactory : ICommonBuildingTemplateFactory;

    public class MineBuildingTemplateFactory : CommonBuildingTemplateFactory, IMineBuildingTemplateFactory
    {
        public MineBuildingTemplateFactory(IBuildingMineKind kind) : base(kind)
        {
        }

        public override IBuildingTemplate CreateBuildingTemplate(int id)
        {
            return new BuildingTemplate
            {
                Actions = [
                    new BuildingAction
                    {
                        Events = [ 
                            new ResourceFamilyManagingHeroEventStrategy {
                                Name = "Hero Mining",
                                Resource = FamilyResourceEnum.Gold,
                                Amount = hero => hero.Stats.Physic * 10
                        }]
                    }
                ],
                Cost = 50,
                Kind = Kind,
                PassiveEvents = [new FamilyResourceManagingEventStrategy
                {
                    Name = "Mining",
                    Resource = FamilyResourceEnum.Gold,
                    ResourceAmount = () => 5
                }]
            };
        }
    }

    public interface IChurchBuildingTemplateFactory : ICommonBuildingTemplateFactory;

    public class ChurchBuildingTemplateFactory : CommonBuildingTemplateFactory, IChurchBuildingTemplateFactory
    {
        public ChurchBuildingTemplateFactory(IBuildingChurchKind kind) : base(kind)
        {
        }

        public override IBuildingTemplate CreateBuildingTemplate(int id)
        {
            return new BuildingTemplate
            {
                Actions = [],
                Cost = 50,
                Kind = Kind
            };
        }
    }

    public interface IInnBuildingTemplateFactory : ICommonBuildingTemplateFactory;

    public class InnBuildingTemplateFactory : CommonBuildingTemplateFactory, IInnBuildingTemplateFactory
    {
        public InnBuildingTemplateFactory(IBuildingInnKind kind) : base(kind)
        {
        }

        public override IBuildingTemplate CreateBuildingTemplate(int id)
        {
            return new BuildingTemplate
            {
                Actions = [],
                Cost = 50,
                Kind = Kind
            };
        }
    }
}
