// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;
using System.Text;

Aes aes = Aes.Create();
aes.GenerateIV();
aes.GenerateKey();

var iv = Convert.ToBase64String(aes.IV);
var key = Convert.ToBase64String(aes.Key);

Console.WriteLine("Salve estas chaves em algum lugar");
Console.WriteLine("IV: {0}", iv);
Console.WriteLine("Key: {0}", key);
Console.WriteLine();

aes.IV = Convert.FromBase64String(iv);
aes.Key = Convert.FromBase64String(key);

var input = 1538;
var serialized = string.Empty;
var encrypted = string.Empty;
var encryptedGuid = Guid.Empty;
var decrypted = string.Empty;
var output = 0;

Console.WriteLine("valor de entrada (int): {0}", input);
Console.WriteLine("Serializando numero inteiro, você pode usa este método para qual quer objeto");
using (var stream = new MemoryStream())
{
    using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
    {
        writer.Write(input);
    }
    var binary = stream.ToArray();
    serialized = Convert.ToBase64String(stream.ToArray());
}
Console.WriteLine("valor serializado: {0}", serialized);
Console.WriteLine();

Console.WriteLine("Encriptando valor serializado");
using (var stream = new MemoryStream())
{
    using (var crypto = new CryptoStream(stream, aes.CreateEncryptor(aes.Key, aes.IV), CryptoStreamMode.Write))
    {
        using (StreamWriter writer = new(crypto))
        {
            writer.Write(serialized);
        }
    }
    var binary = stream.ToArray();
    encrypted = Convert.ToBase64String(binary);
    encryptedGuid = new Guid(binary);
}
Console.WriteLine("valor encriptado (base64): {0}", encrypted);
Console.WriteLine("valor encriptado (guid): {0}", encryptedGuid);
Console.WriteLine("só é possivel usar guid se o valor encryptado tiver 4 bytes, que é o caso dos inteiros");
Console.WriteLine();

Console.WriteLine("Dencriptando valor encriptando");
using (var stream = new MemoryStream())
{
    // ou se preferir trabalhar com base64
    // stream.Write(Convert.FromBase64String(encrypted));
    stream.Write(encryptedGuid.ToByteArray());
    stream.Seek(0, SeekOrigin.Begin);
    stream.Flush();

    
    using (var crypto = new CryptoStream(stream, aes.CreateDecryptor(aes.Key, aes.IV), CryptoStreamMode.Read))
    {
        using (var reader = new StreamReader(crypto))
        {
            decrypted = reader.ReadToEnd();
        }
    }
}
Console.WriteLine("valor dencriptado (base64): {0}", decrypted);
Console.WriteLine("dencriptado e serialized são iguais? {0}: ", decrypted == serialized ? "sim" : "não");
Console.WriteLine();

Console.WriteLine("Deserializando valor dencriptado");
using (var stream = new MemoryStream())
{
    stream.Write(Convert.FromBase64String(decrypted));
    stream.Seek(0, SeekOrigin.Begin);
    stream.Flush();

    using (var writer = new BinaryReader(stream, Encoding.UTF8, false))
    {
        output = writer.ReadInt32();
    }
}
Console.WriteLine("valor deserializado (int): {0}", output);
Console.WriteLine("input e output são iguais? {0}", input == output ? "sim" : "não");
Console.ReadLine();

// 1537 => 837a6ef5-e07c-0586-bb8d-c6e269d9dc96
// 1538 => 6661bf10-af16-f509-1f46-2c9980d04525