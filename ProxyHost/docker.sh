docker build -t wcfhost:lastest .

docker run -itd --name ProxyHost wcfhost

docker inspect -f="{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}" ProxyHost