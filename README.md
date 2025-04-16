# 🔐 CertGen

**CertGen** is a C# utility that generates a self-signed Certificate Authority (CA) using RSA keys with customizable length. It leverages the [BouncyCastle](https://github.com/bcgit/bc-csharp) cryptography library to create and export the certificate in PKCS#12 (`.p12`) format.

## 🔍 Features

- Generates a self-signed certificate with configurable RSA key length.
- Uses **SHA512WITHRSA** signature algorithm.
- Adds blank X509 extensions.
- Supports CLI usage with optional arguments.

## 🚀 Usage

### 🔧 Compile

```sh
csc.exe /nologo /debug- /optimize+ /r:BouncyCastle.Crypto.dll CertGen.cs
```

### ▶️ Run

```sh
CertGen.exe [key_length] [cert_name]
```

- `[key_length]` *(optional)*: RSA key length in bits (default: `2048`)
- `[cert_name]` *(optional)*: Name of the output `.p12` certificate file (default: `MyCA.p12`)

### 💡 Example

```sh
CertGen.exe 4096 Cert.p12
```

## 📜 Notes

- Certificate validity spans ~10 years.
- Ideal for local testing, internal tools, or as a custom root CA.

---

## ⚠️ Disclaimer

This tool is intended for **educational and development purposes** only. The author is not responsible for any misuse.

---

## 📜 License

This project is released under the [MIT License](LICENSE).
