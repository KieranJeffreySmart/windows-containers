# import root cert
Import-Certificate -FilePath 'root-ca.cer' -CertStoreLocation 'Cert:\LocalMachine\Root'
# create binding for ssl
New-WebBinding -Name 'Default Web Site' -IPAddress '*' -Port 443 -Protocol 'https' -Force

# get binding andassign ssl certificate to default website (must be rub as single user and therefor on one line)
$certThumbprint = (Import-PfxCertificate -FilePath 'wildcard.pfx' -Password (ConvertTo-SecureString -String 'Password123' -AsPlainText -Force) -CertStoreLocation 'Cert:\LocalMachine\My').Thumbprint
$binding = (Get-WebBinding -Name 'Default Web Site' -Port 443 -IPAddress '*' -Protocol 'https')
$binding.AddSslCertificate($certThumbprint, 'My')

# enable tsl 1.2
regedit /s  tls12.reg