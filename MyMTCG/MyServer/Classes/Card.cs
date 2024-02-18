namespace MyServer.Classes;

public class Card
{
    public Card(int cardId, string name, string category, int damage, bool spell, string type)
    {
        card_id = cardId;
        Name = name;
        Category = category;
        Damage = damage;
        Spell = spell;
        Type = type;
    }

    public int card_id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public int Damage { get; private set; }
    public bool Spell { get; private set; }
    public string Type { get; private set; }
    
}