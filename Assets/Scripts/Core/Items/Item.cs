using Zenject;

public class Item : Activateable , IItem
{

    public class Factory : PlaceholderFactory<Item> { }
}

