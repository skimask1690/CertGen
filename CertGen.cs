// Compile: csc.exe /nologo /debug- /optimize+ /r:BouncyCastle.Crypto.dll CertGen.cs

using System;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;

public static class CreateCertificate
{
    public static X509Certificate2 CreateCertificateAuthority(int keyLength)
    {
        SecureRandom secureRandom = new SecureRandom();
        RsaKeyPairGenerator rsaKeyPairGenerator = new RsaKeyPairGenerator();
        rsaKeyPairGenerator.Init(new KeyGenerationParameters(secureRandom, keyLength));

        AsymmetricCipherKeyPair asymmetricCipherKeyPair = rsaKeyPairGenerator.GenerateKeyPair();

        X509V3CertificateGenerator x509V3CertificateGenerator = new X509V3CertificateGenerator();

        X509Name issuerDN = new X509Name("CN=");
        X509Name subjectDN = new X509Name("CN=");
        BigInteger serialNumber = BigInteger.ProbablePrime(160, new SecureRandom());

        x509V3CertificateGenerator.SetSerialNumber(serialNumber);
        x509V3CertificateGenerator.SetSubjectDN(subjectDN);
        x509V3CertificateGenerator.SetIssuerDN(issuerDN);
        x509V3CertificateGenerator.SetNotAfter(DateTime.UtcNow.Subtract(new TimeSpan(-3650, 0, 0, 0)));
        x509V3CertificateGenerator.SetNotBefore(DateTime.UtcNow.Subtract(new TimeSpan(285, 0, 0, 0)));
        x509V3CertificateGenerator.SetPublicKey(asymmetricCipherKeyPair.Public);
        x509V3CertificateGenerator.AddExtension(X509Extensions.SubjectKeyIdentifier, false, new SubjectKeyIdentifierStructure(asymmetricCipherKeyPair.Public));
        x509V3CertificateGenerator.AddExtension(X509Extensions.BasicConstraints, true, new BasicConstraints(true));

        ISignatureFactory signatureFactory = new Asn1SignatureFactory("SHA512WITHRSA", asymmetricCipherKeyPair.Private, secureRandom);

        return new X509Certificate2(
            DotNetUtilities.ToX509Certificate(x509V3CertificateGenerator.Generate(signatureFactory)))
        {
            PrivateKey = DotNetUtilities.ToRSA(asymmetricCipherKeyPair.Private as RsaPrivateCrtKeyParameters)
        };
    }
}

class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            string firstArg = args[0].ToLower();
            if (firstArg == "-h" || firstArg == "-help" || firstArg == "--h" || firstArg == "--help" ||
                firstArg == "/h" || firstArg == "/help" || firstArg == "help" || firstArg == "h" ||
                firstArg == "-?" || firstArg == "?" || firstArg == "/?")
            {
                Console.WriteLine("\nUsage: " + AppDomain.CurrentDomain.FriendlyName + " [key_length] [cert_name]");
                return;
            }
        }

        string name = "MyCA.p12";
        int keyLength = 2048;

        if (args.Length >= 1)
        {
            if (!int.TryParse(args[0], out keyLength))
            {
                Console.WriteLine("\nError: Invalid key length. Please provide a valid integer.");
                return;
            }

            if (keyLength < 745)
            {
                Console.WriteLine("\nError: Key length must be 745 bits or greater.");
                return;
            }
        }

        if (args.Length >= 2)
        {
            name = args[1].EndsWith(".p12", StringComparison.OrdinalIgnoreCase) ? args[1] : args[1] + ".p12";
        }

        try
        {
            X509Certificate2 cert = CreateCertificate.CreateCertificateAuthority(keyLength);
            byte[] p12Data = cert.Export(X509ContentType.Pkcs12);
            System.IO.File.WriteAllBytes(name, p12Data);

            Console.WriteLine("\nCertificate generated successfully!\n");
            Console.WriteLine("RSA key length: " + keyLength + " bits");
            Console.WriteLine("Saved as: " + name + "");
        }
        catch (Exception ex)
        {
            Console.WriteLine("\nError generating certificate: " + ex.Message);
        }
    }
}
