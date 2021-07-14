# Collaborative virtual reality integrating inverse kinematics
 
 This is the software i developed for my Bachelor’s Final Thesis.

It's a VR environment developed using Unity3D.
The pourpose of this environment was to realise a series of experiments about how people behave while using VR, and the differences of the behaviour they show between reality and VR.
In this environment, there are different types of user representation so it could also be studied how the accuracity of the user's representation affected their behaviour.

This repo contains the full unity3D project, and also a [compressed build](https://github.com/YerayBB/Collaborative-virtual-reality-integrating-inverse-kinematics/blob/main/Build.rar), ready to execute once decompressed.

To use this VR software the required VR devices are: 
1. VR Headset
2. 2 HTC vive Controllers: one per hand
3. 3 vive trackers:
    * the first secured on the lower back with the light pointing up.
    * the second in the left instep with the light pointing to the toes.
    * the third in the left instep with the light pointing to the toes.
If you dont have all the required devices, the software contains a VR device simulator, so it can ve tested without any device. But since it uses 6 devices, the controll and set up of them all in the simulator can be complex to the user.

Since this software was designed to realise experiments, all the start up configuration for each sesion is made from the computer screen so the experiment participants won't be implied.
## Computer interface
The different menus of the interface are connected following this map
![UI map image](https://github.com/YerayBB/Collaborative-virtual-reality-integrating-inverse-kinematics/blob/main/Documentation/Images/Interface%20map.jpg)
### Menu explanation
1. Menu to choose between realise a sesion or visualize data from a previous sesion in the scene.
2. To begin a experiment first is selected if its gonna be single or multiplayer. In this menu are shown the connection options and the current status of the connection.
3. In this menu is choosen which file will be visualized, if an obstacle must be shown (for single user experiements) and if the data is read automatically or only when a key is pressed.
4. Screen with the options for a single participant experiment. It has options to create an obstacle, what kind of representation will be used for the participant and the options to configure the data collection for this sesion.
5. Screen with the options for the master version of the two participants experiment. Contains the same options that the menu for single user (4), except the posibility to create an obstacle.
6. Waiting screen for the user that joins the room (client) in the experiment for two participants. This menu is just to make sure that the client does not start a sesion without getting the configuration from the master.
7. Main menu when the configuration for the sesion is completed or the data visualizer has started. Contains the controls information for the camera system and the data collector. Also has 2 buttons, one to restart the software and the other to close the software.
8. One of the cameras to see the scene, this one shows the scene from the top.
9. Another of the cameras to see the scene, this one shows the scene from one of the sides of the room.
10. Last of the cameras to see the scene, this one shows the scene from the other side of the room.
11. The whole camera system, in this screen all the cameras are on display at the same time, it also shows a brief description of the camera system controls.

* If you are using this software using the vr device simulator, to be able to see in the computer screen through the simulated VR headset you have to press the key *K*. To go back to the usual computer menu press *K* again.


## NOTES
* Even though there is the option to use this software in multiplayer mode, this software was not designed with the idea of having more than two running at the same time, which means it only has the capacity of creating one room, if someone else has a room open it may produce errors to create another.
