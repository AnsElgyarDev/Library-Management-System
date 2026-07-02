namespace Practice.Helpers;
public static class SerialGenerator
{
    public static string GenerateSerialNumber()
    {
        Int64 serialNumber = Random.Shared.NextInt64(1000, 99999);
        return $"BK-{serialNumber}";
    }
}