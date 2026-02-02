namespace Models;

public class Conversation
{
    public int Id {get; set;}
}


public class Conversation_participants
{
    public int Conversation_id {get; set;}
    public int User_id {get; set;}
    
}