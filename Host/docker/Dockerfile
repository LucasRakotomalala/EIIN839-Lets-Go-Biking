FROM mcr.microsoft.com/dotnet/framework/wcf:4.8

RUN powershell -Command Add-WindowsFeature NET-WCF-HTTP-Activation45

WORKDIR C://Host

EXPOSE 8080

COPY bin/ .

ENTRYPOINT ["Debug/Host.exe"] 