# IoT Data Collection
## KF7014 Advanced Programming - Practical Assessment 
This project is a team work done for a level 7 University module in 2023/24. It has earned a mark of 74% (first class in UK University marking system).

It's focus is a web application, which displays weather data retrieved from microservices which hold measurements of distinct aspects of weather. Several different views are available on a dashboard accessible to users who first log in, including both individual and collated reports, as well as a visualised history of weather measurements. The assessment was an exercise in software architecture and required us to properly structure our program and employ a range of architecture and design patterns.

Team members:
* Marek Kočvara
* [Vincent Briat](https://github.com/vincentbriat)
* [Cosmin Bianu](https://github.com/cosmin-bianu)

## Project Description
This application serves its user to view (mock) weather data collected by IoT sensors in a human-readable way. This data comes from three microservices, one for precipitation, one for temperature, and the last for humidity data. These microservices expose their data via a RESTful API. The client web application fetches this data, processes it, and displays it on a dashboard. 

The dashboard is modularised by using partial razor views, with each section representing a different view of the data. The first three sections display the latest data taken from each of the microservices individually. These parts work independently of each other and their respective data-providing microservices. The next part displays the latest aggregate data, which shows collated data from all three microservices, for the latest available date. Finally, the last section visualises aggregated historical data on an interactive chart. 

To view the dashboard, one must first authenticate by logging in. The authentication system utilises ASP.NET Identity and keeps user data in a MySQL database. 

An API gateway sits between the client and data layer, which routes all requests from the client application to the relevant microservices. It is an implementation of an Ocelot gateway. This introduces a layer of separation between the client app and the microservices and obfuscates the microservices’ endpoints to the client.  

Domain events within the application are handled by an event bus implemented using the EasyNetQ package, which in turn utilises a RabbitMQ server to handle queues, subscriptions, and publications. Some of the partial views that make up the client app’s dashboard are subscribed to severe weather alert events, which are published by the microservices when detected. 

The entire application is containerised and deployed using Docker. Docker handles ports at which the app can be accessed, as well as internal communication between the client, gateway, and microservices. It allows for network segregation to isolate the microservices from the external hosts. This way, authorisation can be handled in a single point (the gateway). 

## Architecture Diagram
![Architecture Diagram drawio](https://github.com/mkocvara/KF7014-Assessment/assets/28301057/5bf99a07-d549-4008-a7ba-a3712132be35)

## Screenshots of Running Application
_Login page:_
![Log-in-page](https://github.com/mkocvara/KF7014-Assessment/assets/28301057/f35bcdeb-1c20-44eb-a637-57deb4d6b9bf)

_Registration page:_
![Register](https://github.com/mkocvara/KF7014-Assessment/assets/28301057/f6897fa2-f765-427c-b2d8-5a3671cecb88)

_Dashboard:_
![Dashboard1](https://github.com/mkocvara/KF7014-Assessment/assets/28301057/0e4404b2-1a59-404d-afd1-a6a7e277bd69)
![Dashboard2](https://github.com/mkocvara/KF7014-Assessment/assets/28301057/b529aa46-7db8-4dd1-b005-25485b5e1544)
![Dashboard3](https://github.com/mkocvara/KF7014-Assessment/assets/28301057/97612a97-5a39-4b73-a864-d56c8bf4a91f)
![Dashboard4](https://github.com/mkocvara/KF7014-Assessment/assets/28301057/cf46cd64-477b-4803-8141-165027ac6630)
![Dashboard5](https://github.com/mkocvara/KF7014-Assessment/assets/28301057/e33de461-f5de-4924-91ab-1f68aad0a8dd)
