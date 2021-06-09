using TowerGenerator;

public class ConfigStairsDown : GeneratorConfigBase
{
    public override GeneratorBase GetGenerator()
    {
        if(_generator == null)
            _generator = new GeneratorStairsDown(this);
        return _generator;
    }

    private GeneratorStairsDown _generator;
}