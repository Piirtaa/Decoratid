using Decoratid.Idioms.Communicating;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Utils
{
    public static class CertUtil
    {
        #region Export Calls
        /// <summary>
        /// Export a certificate to a PEM format string
        /// </summary>
        /// <param name="cert">The certificate to export</param>
        /// <returns>A PEM encoded string</returns>
        public static string ExportToPEM(System.Security.Cryptography.X509Certificates.X509Certificate cert)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("-----BEGIN CERTIFICATE-----");
            builder.AppendLine(Convert.ToBase64String(cert.Export(X509ContentType.Cert), Base64FormattingOptions.InsertLineBreaks));
            builder.AppendLine("-----END CERTIFICATE-----");

            return builder.ToString();
        }
        public static void SaveAsCER(X509Certificate2 cert, string path)
        {
            File.WriteAllBytes(path, cert.Export(X509ContentType.Cert));
        }

        //public static void SaveAsPVK(X509Certificate2 cert, string path)
        //{
        //    File.WriteAllBytes(path, cert.Export(X509ContentType.Pkcs12, (string)null));
        //}
        //public static void SaveToPFX(X509Certificate newCert,
        //  AsymmetricCipherKeyPair kp,
        //  string FilePath,
        //  string CertAlias,
        //  string Password)
        //{
        //              //Org.BouncyCastle.X509.X509Certificate newCert,

        //    var newStore = new Pkcs12Store();

        //    var certEntry = new X509CertificateEntry(newCert);

        //    newStore.SetCertificateEntry(
        //        CertAlias,
        //        certEntry
        //        );

        //    newStore.SetKeyEntry(
        //        CertAlias,
        //        new AsymmetricKeyEntry(kp.Private),
        //        new[] { certEntry }
        //        );

        //    using (var certFile = File.Create(FilePath))
        //    {
        //        newStore.Save(
        //            certFile,
        //            Password.ToCharArray(),
        //            new SecureRandom(new CryptoApiRandomGenerator())
        //            );
        //    }
        //}
        #endregion
        //        public static void dfsdfoui9()
        //        {
        //            //X509Certificate2 cert;

        //            //save to file
        //            var cert = new X509Certificate2(bytes, password, X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);

        //            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
        //store.Open(OpenFlags.ReadWrite);
        //store.Add(cert);
        //store.Close();

        //            //key goes to C:\Users\Paul\AppData\Roaming\Microsoft\SystemCertificates\My\Keys\62207B818FC553C92CC6D2C2F869603C190544FB 

        //            //var file = Path.Combine(Path.GetTempPath(), "Octo-" + Guid.NewGuid());
        //            try
        //            {
        //                File.WriteAllBytes(file, bytes);
        //                return new X509Certificate2(file, /* ...options... */);
        //            }
        //            finally
        //            {
        //                File.Delete(file);
        //            }

        //            store.Remove(cert);
        //        }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="subjectName">eg. "CN=127.0.01"</param>
        ///// <param name="durationInMins"></param>
        ///// <param name="issuerName">eg. "CN=MYTESTCA"</param>
        //public static X509Certificate2 GenerateAndRegisterSelfSignedCertificate(string subjectName, int durationInMins, string issuerName)
        //{
        //    try
        //    {
        //        AsymmetricKeyParameter myCAprivateKey = null;
        //        //generate a root CA cert and obtain the privateKey
        //        X509Certificate2 MyRootCAcert = GenerateCACertificate(issuerName, durationInMins, ref myCAprivateKey);
        //        //add CA cert to store
        //        AddCertToStore(MyRootCAcert, StoreName.Root, StoreLocation.LocalMachine);

        //        //generate cert based on the CA cert privateKey
        //        X509Certificate2 MyCert = GenerateSelfSignedCertificate(subjectName, durationInMins, issuerName, myCAprivateKey);
        //        //add cert to store
        //        AddCertToStore(MyCert, StoreName.My, StoreLocation.LocalMachine);
        //        return MyCert;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}


        //public static X509Certificate2 GenerateSelfSignedCertificate(string subjectName, int durationInMins, string issuerName, AsymmetricKeyParameter issuerPrivKey)
        //{
        //    const int keyStrength = 2048;

        //    // Generating Random Numbers
        //    CryptoApiRandomGenerator randomGenerator = new CryptoApiRandomGenerator();
        //    SecureRandom random = new SecureRandom(randomGenerator);

        //    // The Certificate Generator
        //    X509V3CertificateGenerator certificateGenerator = new X509V3CertificateGenerator();

        //    // Serial Number
        //    BigInteger serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random);
        //    certificateGenerator.SetSerialNumber(serialNumber);

        //    // Signature Algorithm
        //    const string signatureAlgorithm = "SHA256WithRSA";
        //    certificateGenerator.SetSignatureAlgorithm(signatureAlgorithm);

        //    // Issuer and Subject Name
        //    X509Name subjectDN = new X509Name(subjectName);
        //    X509Name issuerDN = new X509Name(issuerName);
        //    certificateGenerator.SetIssuerDN(issuerDN);
        //    certificateGenerator.SetSubjectDN(subjectDN);

        //    // Valid For
        //    DateTime notBefore = DateTime.UtcNow.Date;
        //    DateTime notAfter = notBefore.AddMinutes(durationInMins);

        //    certificateGenerator.SetNotBefore(notBefore);
        //    certificateGenerator.SetNotAfter(notAfter);

        //    // Subject Public Key
        //    AsymmetricCipherKeyPair subjectKeyPair;
        //    var keyGenerationParameters = new KeyGenerationParameters(random, keyStrength);
        //    var keyPairGenerator = new RsaKeyPairGenerator();
        //    keyPairGenerator.Init(keyGenerationParameters);
        //    subjectKeyPair = keyPairGenerator.GenerateKeyPair();

        //    certificateGenerator.SetPublicKey(subjectKeyPair.Public);

        //    // Generating the Certificate
        //    AsymmetricCipherKeyPair issuerKeyPair = subjectKeyPair;

        //    // selfsign certificate
        //    Org.BouncyCastle.X509.X509Certificate certificate = certificateGenerator.Generate(issuerPrivKey, random);

        //    // correcponding private key
        //    PrivateKeyInfo info = PrivateKeyInfoFactory.CreatePrivateKeyInfo(subjectKeyPair.Private);


        //    // merge into X509Certificate2
        //    X509Certificate2 x509 = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificate.GetEncoded());

        //    Asn1Sequence seq = (Asn1Sequence)Asn1Object.FromByteArray(info.PrivateKey.GetDerEncoded());
        //    if (seq.Count != 9)
        //    {
        //        //throw new PemException("malformed sequence in RSA private key");
        //    }

        //    RsaPrivateKeyStructure rsa = new RsaPrivateKeyStructure(seq);
        //    RsaPrivateCrtKeyParameters rsaparams = new RsaPrivateCrtKeyParameters(
        //        rsa.Modulus, rsa.PublicExponent, rsa.PrivateExponent, rsa.Prime1, rsa.Prime2, rsa.Exponent1, rsa.Exponent2, rsa.Coefficient);

        //    x509.PrivateKey = DotNetUtilities.ToRSA(rsaparams);
        //    return x509;

        //}
    //    /// <summary>
    //    /// builds the boss cert
    //    /// </summary>
    //    /// <param name="subjectName"></param>
    //    /// <param name="CaPrivateKey"></param>
    //    /// <returns></returns>
    //    public static X509Certificate2 GenerateCACertificate(string subjectName, int durationInMins, out AsymmetricCipherKeyPair kp)
    //    {
    //        const int keyStrength = 2048;

    //        // Generating Random Numbers
    //        CryptoApiRandomGenerator randomGenerator = new CryptoApiRandomGenerator();
    //        SecureRandom random = new SecureRandom(randomGenerator);

    //        // The Certificate Generator
    //        X509V3CertificateGenerator certificateGenerator = new X509V3CertificateGenerator();


    //        var certName = new X509Name("CN=" + subjectName);
    //        BigInteger serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(Int64.MaxValue), random);

    //        certificateGenerator.SetSerialNumber(serialNumber);
    //        certificateGenerator.SetSubjectDN(certName);
    //        certificateGenerator.SetIssuerDN(certName);
    //        certificateGenerator.SetNotBefore(DateTime.Now);
    //        certificateGenerator.SetNotAfter(DateTime.Now.AddMinutes(durationInMins));
    //        certificateGenerator.SetSignatureAlgorithm("SHA256WithRSA");

    //        var kpgen = new RsaKeyPairGenerator();
    //        // certificate strength 1024 bits
    //        kpgen.Init(new KeyGenerationParameters(random, keyStrength));
    //        kp = kpgen.GenerateKeyPair();
    //        certificateGenerator.SetPublicKey(kp.Public);

    //        certificateGenerator.AddExtension(
    //X509Extensions.AuthorityKeyIdentifier.Id,
    //false,
    //new AuthorityKeyIdentifier(
    //    SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(kp.Public),
    //    new GeneralNames(new GeneralName(certName)),
    //    serialNumber));

    //        /* 
    //         1.3.6.1.5.5.7.3.1 - id_kp_serverAuth 
    //         1.3.6.1.5.5.7.3.2 - id_kp_clientAuth 
    //         1.3.6.1.5.5.7.3.3 - id_kp_codeSigning 
    //         1.3.6.1.5.5.7.3.4 - id_kp_emailProtection 
    //         1.3.6.1.5.5.7.3.5 - id-kp-ipsecEndSystem 
    //         1.3.6.1.5.5.7.3.6 - id-kp-ipsecTunnel 
    //         1.3.6.1.5.5.7.3.7 - id-kp-ipsecUser 
    //         1.3.6.1.5.5.7.3.8 - id_kp_timeStamping 
    //         1.3.6.1.5.5.7.3.9 - OCSPSigning
    //         */
    //        certificateGenerator.AddExtension(
    //            X509Extensions.ExtendedKeyUsage.Id,
    //            false,
    //            new ExtendedKeyUsage(new[] { KeyPurposeID.IdKPServerAuth }));

    //        var newCert = certificateGenerator.Generate(kp.Private);

    //        return newCert;


    //        //// Generating the Certificate
    //        //AsymmetricCipherKeyPair issuerKeyPair = subjectKeyPair;

    //        //// selfsign certificate
    //        //Org.BouncyCastle.X509.X509Certificate certificate = certificateGenerator.Generate(issuerKeyPair.Private, random);
    //        //X509Certificate2 x509 = new System.Security.Cryptography.X509Certificates.X509Certificate2(certificate.GetEncoded());

    //        //CaPrivateKey = issuerKeyPair.Private;

    //        //return x509;

    //        //return issuerKeyPair.Private;
    //    }
        
        public static System.Security.Cryptography.X509Certificates.X509Certificate2 GetCert(string certName, System.Security.Cryptography.X509Certificates.StoreName st, System.Security.Cryptography.X509Certificates.StoreLocation sl)
        {
            System.Security.Cryptography.X509Certificates.X509Certificate2 cert = null;

            try
            {
                X509Store store = new X509Store(st, sl);
                store.Open(OpenFlags.ReadOnly);

                X509Certificate2Collection certs =
            store.Certificates.Find(X509FindType.FindBySubjectName,
            certName,
            true);

                if (certs.Count > 0)
                {
                    cert = certs[0];
                }
                store.Close();
            }
            catch
            {
            }

            return cert;
        }
        /// <summary>
        /// registers a cert with a cert store
        /// </summary>
        /// <param name="cert"></param>
        /// <param name="st"></param>
        /// <param name="sl"></param>
        /// <returns></returns>
        public static bool AddCertToStore(System.Security.Cryptography.X509Certificates.X509Certificate2 cert, System.Security.Cryptography.X509Certificates.StoreName st, System.Security.Cryptography.X509Certificates.StoreLocation sl)
        {
            bool bRet = false;

            try
            {
                
                X509Store store = new X509Store(st, sl);
                store.Open(OpenFlags.ReadWrite);
                store.Add(cert);
                store.Close();


                bRet = true;
            }
            catch
            {
            }

            return bRet;
        }

        public static bool RemoveCertFromStore(System.Security.Cryptography.X509Certificates.X509Certificate2 cert, System.Security.Cryptography.X509Certificates.StoreName st, System.Security.Cryptography.X509Certificates.StoreLocation sl)
        {
            bool bRet = false;

            try
            {
                X509Store store = new X509Store(st, sl);
                store.Open(OpenFlags.ReadWrite);
                store.Remove(cert);
                store.Close();
                bRet = true;
            }
            catch
            {
            }

            return bRet;
        }

    }

}
