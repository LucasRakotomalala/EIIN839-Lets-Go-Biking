# Start with Windows Server Core image
FROM mcr.microsoft.com/dotnet/framework/wcf:4.8

RUN powershell -Command Add-WindowsFeature NET-WCF-HTTP-Activation45

# Creates a directory for the Host
WORKDIR Proxy

# Listen on port 83.
EXPOSE 8733

# Copy the WCF host into the container.
COPY Bin/ .

ENTRYPOINT BasicHttpHost.exe