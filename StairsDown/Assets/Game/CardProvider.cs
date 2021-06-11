using GameLib.Random;
using TowerGenerator;
using UnityEngine;
using UnityEngine.Assertions;

public class CardProvider : MonoBehaviour
{
    public CardSpawnPoint[] Cards;


    public MetaProvider MetaProvider;
    private IPseudoRandomNumberGenerator _rnd = RandomHelper.CreateRandomNumberGenerator();

    public CardSpawnPoint GetCard(int card)
    {
        return Cards[card];
    }

    public MetaBase GetRandomMeta()
    {
        Assert.IsTrue(MetaProvider.Metas.Length != 0);
        return _rnd.FromEnumerable(MetaProvider.GetMetas());
    }
}
