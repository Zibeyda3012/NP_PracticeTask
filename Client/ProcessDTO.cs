namespace Client;

public record ProcessDTO(int Id, string Name)
{
    public override string ToString()
    {
        return $"{Id}.{Name}";
    }
}

