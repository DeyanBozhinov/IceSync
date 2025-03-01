# IceSync

1. Setup up appsettings.json file (AES encryption - key have specific byte length)
2. Run migrations to initialize database

# Client-app

1. Install Node.js if it's missing
2. Run "npm install" to install packages
3. Setup BASE_URL variable (by default it uses http)

# Improvements
1. IMemoryCache is not distributed caching service. If there are multiple instances of the application, the cache won't be shared between them. Additional requests would be made.
2. The background service will start on every instance of the application. A mechanism for its management needs to be implemented (e.g., through a configuration file and a table      in the database).
3. DataCore and Services should be moved to Class Library projects for better architectural structure.

