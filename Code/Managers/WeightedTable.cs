using System.Collections.Generic;
using Godot;

namespace HellFarm.Code.Managers;

public partial class WeightedTable
{
    private List<EnemyItem> _items = new List<EnemyItem>();
    private int _weightSum = 0;
    
    public void AddItem(EnemyItem item)
    {
        _items.Add(item);
        _weightSum += item.Weight;
    }

    public EnemyItem PickItem()
    {
        var chosenWeight = GD.RandRange(1, _weightSum);
        var iterationSum = 0;
        foreach (var item in _items)
        {
            iterationSum += item.Weight;
            if (chosenWeight <= iterationSum)
            {
                return item;
            }
        }
        return null;
    }
}

public class EnemyItem
{
    public int Weight { get; set; }
    public PackedScene EnemyScene { get; set; }
}
