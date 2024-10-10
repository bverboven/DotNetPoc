# Self hosting (minimal) API

Minimal API that can be registered as a Windows Service on a configurable port.

Generate key & certificate
```
openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 365
```

Generate pfx
```
openssl pkcs12 -export -out certificate.pfx -inkey key.pem -in cert.pem
```

