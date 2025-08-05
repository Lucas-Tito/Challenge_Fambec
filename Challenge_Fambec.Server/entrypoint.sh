#!/bin/bash
set -e

# Define the path for the certificate
CERT_FILE="/https/aspnetapp.pfx"
# Get the password from the environment variable set in docker-compose.yml
CERT_PASSWORD=$ASPNETCORE_Kestrel__Certificates__Default__Password

# Check if the certificate file does not exist
if [ ! -f "$CERT_FILE" ]; then
    echo "HTTPS certificate not found. Creating a new one..."
    # Create a new certificate. The password is required.
    dotnet dev-certs https -ep "$CERT_FILE" -p "$CERT_PASSWORD"
else
    echo "HTTPS certificate found."
fi

# Execute the main command that was passed to the script (e.g., "dotnet watch run")
exec "$@"