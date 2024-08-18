using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance = null;

    public enum EatingFoods { None, Crumb, Blueberry, Strawberry }
    public EatingFoods ActiveEatableFood { get; set; } = EatingFoods.Crumb;

    public int CollectableCount { get; set; } = 0;
    public bool AtMaxEatingStage => CollectableCount >= 3;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CollectableCount = 0;
        ActiveEatableFood = EatingFoods.Crumb;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void UpgradeFoodType()
    {
        //Already at max eatable
        if (ActiveEatableFood == EatingFoods.Strawberry)
            return;

        ActiveEatableFood++;
    }
}
