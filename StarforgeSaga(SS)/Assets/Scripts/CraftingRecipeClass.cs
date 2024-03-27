using UnityEngine;
[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "Crafting/Recipe")]
public class CraftingRecipeClass : ScriptableObject
{
    public Slot[] inputItems;
    public Slot outItem;

    public bool CanCraft(InventoryManager inventory)
    {
        //check if you hve free space in eq
        if(inventory.isFull())
            return false;

        for(int i = 0; i < inputItems.Length; i++)
        {
            if (!inventory.Containts(inputItems[i].GetItem(), inputItems[i].GetCount()))
                return false;
        }

        return true;
    }
    public void Craft(InventoryManager inventory)
    {
        //remove items from inventory
        for(int i = 0; i < inputItems.Length; i++)
        {
            inventory.Remove(inputItems[i].GetItem(), inputItems[i].GetCount());
            Debug.Log("usun");
        }
        //add craft item
        inventory.Add(outItem.GetItem(), outItem.GetCount());
        Debug.Log("stworz");
    }
}