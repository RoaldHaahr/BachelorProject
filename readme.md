BEFORE RUNNING DO:

- install mongodb (https://www.mongodb.com/download-center#community)
- goto mongo folder (C:\Program Files\MongoDB\Server\3.2\bin) and open cmd
- run mongod
- open new cmd
- run mongo
- type 
	use admin
	db.system.users.remove({})    <== removing all users
	db.system.version.remove({}) <== removing current version 
	db.system.version.insert({ "_id" : "authSchema", "currentVersion" : 3 })
	use blog
	db.createUser({ "user" : "rh", "pwd" : "1234", "roles" : [{ "role" : "readWrite", "db" : "blog" }]})