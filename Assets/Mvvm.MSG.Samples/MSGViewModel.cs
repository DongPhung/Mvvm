using System.Collections;
using System.Collections.Generic;
using Mvvm;
using UnityEngine;
using ZBase.Foundation.Mvvm.ComponentModel;
using ZBase.Foundation.Mvvm.Unity.ViewBinding;

public partial class MSGViewModel : MonoBehaviour, IObservableObject
{
    [ObservableProperty]
    public int Score { get => Get_Score(); set => Set_Score(value); }

    [ObservableProperty]
    public ItemModel ItemModelA { get => Get_ItemModelA(); set => Set_ItemModelA(value); }

    [ObservableProperty]
    private ItemModel _itemModelB;
    public MonoBindingContext itemContext;

    void Awake()
    {
        ItemModelA = new ItemModel();
    }
    // Start is called before the first frame update
    void Start()
    {
        Score = 0;
    }
    
    [ContextMenu("Add Score")]
    void AddScore()
    {
        Score += 10;
    }

    [ContextMenu("New Item A")]
    void GenNewItemA()
    {
        int frame = Time.frameCount;
        
        ItemModelA = new ItemModel();
        ItemModelA.BasicInfo.Name = "Item A: " + frame;
        ItemModelA.BasicInfo.Desciption = "Item A DES: " + frame;
        ItemModelA.Owned = frame + 10;
        ItemModelA.Used = frame + 20;
        ItemModelA.Cost = frame + 30;
    }
    
    [ContextMenu("Update Item A")]
    void UpdateItemA()
    {
        int frame = Time.frameCount;
        
        ItemModelA.BasicInfo.Name = "Item A_: " + frame;
        ItemModelA.BasicInfo.Desciption = "Item A_ DES: " + frame;
        ItemModelA.Owned = frame + 100;
        ItemModelA.Used = frame + 200;
        ItemModelA.Cost = frame + 300;
    }
    
    [ContextMenu("Item B")]
    void GenNewItemB()
    {
        int frame = Time.frameCount;
        
        ItemModelB = new ItemModel();
        ItemModelB.BasicInfo.Name = "Item B: " + frame;
        ItemModelB.BasicInfo.Desciption = "Item B DES: " + frame;
        ItemModelB.Owned = frame + 50;
        ItemModelB.Used = frame + 80;
        ItemModelB.Cost = frame * 10;
        itemContext.InitializeManually(_itemModelB);
    }
    
    [ContextMenu("Update Item B")]
    void UpdateItemB()
    {
        int frame = Time.frameCount;
        
        ItemModelB.BasicInfo.Name = "Item B__: " + frame;
        ItemModelB.BasicInfo.Desciption = "Item B__ DES: " + frame;
        ItemModelB.Owned = frame + 300;
        ItemModelB.Used = frame + 400;
        ItemModelB.Cost = frame * 10;
    }
}
