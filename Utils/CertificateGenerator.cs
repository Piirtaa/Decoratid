using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decoratid.Utils
{
    /// <summary>
    /// Based on:
    /// http://stackoverflow.com/questions/3770233/is-it-possible-to-programmatically-generate-an-x509-certificate-using-only-c
    /// http://web.archive.org/web/20100504192226/http://www.fkollmann.de/v2/post/Creating-certificates-using-BouncyCastle.aspx
    /// </summary>
    public class CertificateGenerator
    {
        public static X509Certificate
            GenerateCertificate(string subjectName, out AsymmetricCipherKeyPair kp)
        {
            var kpgen = new RsaKeyPairGenerator();

            // certificate strength 1024 bits
            kpgen.Init(new KeyGenerationParameters(
                  new SecureRandom(new CryptoApiRandomGenerator()), 1024));

            kp = kpgen.GenerateKeyPair();

            var gen = new X509V3CertificateGenerator();

            var certName = new X509Name("CN=" + subjectName);
            var serialNo = BigInteger.ProbablePrime(120, new Random());

            gen.SetSerialNumber(serialNo);
            gen.SetSubjectDN(certName);
            gen.SetIssuerDN(certName);
            gen.SetNotAfter(DateTime.Now.AddYears(100));
            gen.SetNotBefore(DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0)));
            gen.SetSignatureAlgorithm("SHA1withRSA");
            gen.SetPublicKey(kp.Public);

            gen.AddExtension(
                X509Extensions.AuthorityKeyIdentifier.Id,
                false,
                new AuthorityKeyIdentifier(
                    SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(kp.Public),
                    new GeneralNames(new GeneralName(certName)),
                    serialNo));

            /* 
             1.3.6.1.5.5.7.3.1 - id_kp_serverAuth 
             1.3.6.1.5.5.7.3.2 - id_kp_clientAuth 
             1.3.6.1.5.5.7.3.3 - id_kp_codeSigning 
             1.3.6.1.5.5.7.3.4 - id_kp_emailProtection 
             1.3.6.1.5.5.7.3.5 - id-kp-ipsecEndSystem 
             1.3.6.1.5.5.7.3.6 - id-kp-ipsecTunnel 
             1.3.6.1.5.5.7.3.7 - id-kp-ipsecUser 
             1.3.6.1.5.5.7.3.8 - id_kp_timeStamping 
             1.3.6.1.5.5.7.3.9 - OCSPSigning
             */
            gen.AddExtension(
                X509Extensions.ExtendedKeyUsage.Id,
                false,
                new ExtendedKeyUsage(new[] { KeyPurposeID.IdKPServerAuth }));

            var newCert = gen.Generate(kp.Private);

            return newCert;
        }

        public static void SaveToFile(
            X509Certificate newCert,
            AsymmetricCipherKeyPair kp,
            string FilePath,
            string CertAlias,
            string Password)
        {
            var newStore = new Pkcs12Store();

            var certEntry = new X509CertificateEntry(newCert);

            newStore.SetCertificateEntry(
                CertAlias,
                certEntry
                );

            newStore.SetKeyEntry(
                CertAlias,
                new AsymmetricKeyEntry(kp.Private),
                new[] { certEntry }
                );

            using (var certFile = File.Create(FilePath))
            {
                newStore.Save(
                    certFile,
                    Password.ToCharArray(),
                    new SecureRandom(new CryptoApiRandomGenerator())
                    );
            }
        }
    }

    public class CertificationTester
    {
        /// <summary>
        /// Are we capable of generating, saving, loading and removing a cert.  A test of a host system's compatibility
        /// with our cert stuff
        /// </summary>
        public static void GenerateSaveAndReload()
        {
            //USING BOUNCY CASTLE
            // create bouncy castle cert and save it
            AsymmetricCipherKeyPair kp;
            var x509 = CertificateGenerator.GenerateCertificate("Subject", out kp);

            string FilePath = "cert.pfx";
            string Alias = "foo";
            string Pwd = "bar";

            // save it
            CertificateGenerator.SaveToFile(x509, kp, FilePath, Alias, Pwd);

            //USING NATIVE.NET TO CONSUME BOUNCY
            // open the store as X509Certificate2
            var x5092 = new System.Security.Cryptography.X509Certificates.X509Certificate2(FilePath, Pwd);

            Console.WriteLine(x5092.SubjectName);
            Console.WriteLine(x5092.Thumbprint);
            Console.WriteLine(x5092.PrivateKey.SignatureAlgorithm);

            //clean it up
            File.Delete(FilePath);
        }

    }
}
