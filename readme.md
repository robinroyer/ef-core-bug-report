Sample Project for bug reporting
====


## Issue

Can not save 0 as a value for an owned entity if the the entity is a new one.


## Set up

Tested with a mariadb container:
```
docker run -p 127.0.0.1:3306:3306 -e MYSQL_ROOT_PASSWORD=root -d mariadb
```

1. No update to 0:


```
➜  Sample git:(master) dotnet run
creating the value to : 666
====
setting the value to : 0
the value is now: 666 (1 values in db)
====
setting the value to : 1
the value is now: 1 (1 values in db)
====
setting the value to : 0
the value is now: 1 (1 values in db)
====
[WITHOUT CREATING A NEW OBJECT] setting the value to : 0
the value is now: 0 (1 values in db)

```

2. when uncommenting ` = DateTime.UtcNow;`: Still no update to 0 but not the same behavior

```
➜  Sample git:(master) dotnet run
creating the value to : 666
====
setting the value to : 666
the value is now: 666 (1 values in db)
====
setting the value to : 1
the value is now: 1 (1 values in db)
====
setting the value to : 1
the value is now: 1 (1 values in db)
====
[WITHOUT CREATING A NEW OBJECT] setting the value to : 0
the value is now: 0 (1 values in db)

```
