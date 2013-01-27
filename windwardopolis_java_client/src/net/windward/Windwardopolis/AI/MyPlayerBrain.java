package net.windward.Windwardopolis.AI;


// Created by Windward Studios, Inc. (www.windward.net). No copyright claimed - do anything you want with this code.


import net.windward.Windwardopolis.api.Company;
import net.windward.Windwardopolis.api.Map;
import net.windward.Windwardopolis.api.Passenger;
import net.windward.Windwardopolis.api.Player;

import javax.imageio.ImageIO;
import java.awt.*;
import java.awt.image.BufferedImage;
import java.awt.image.DataBufferByte;
import java.awt.image.WritableRaster;
import java.io.DataInputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;

/**
 * The sample C# AI. Start with this project but write your own code as this is a very simplistic implementation of the AI.
 */
public class MyPlayerBrain implements net.windward.Windwardopolis.AI.IPlayerAI {
    // bugbug - put your team name here.
    private static String NAME = "MODULUS";

    // bugbug - put your school name here. Must be 11 letters or less (ie use MIT, not Massachussets Institute of Technology).
    public static String SCHOOL = "CSM";

    /**
     * The name of the player.
     */
    private String privateName;

    public final String getName() {
        return privateName;
    }

    private void setName(String value) {
        privateName = value;
    }

    /**
     * The game map.
     */
    private Map privateGameMap;

    public final Map getGameMap() {
        return privateGameMap;
    }

    private void setGameMap(Map value) {
        privateGameMap = value;
    }

    /**
     * All of the players, including myself.
     */
    private java.util.ArrayList<Player> privatePlayers;

    public final java.util.ArrayList<Player> getPlayers() {
        return privatePlayers;
    }

    private void setPlayers(java.util.ArrayList<Player> value) {
        privatePlayers = value;
    }

    /**
     * All of the companies.
     */
    private java.util.ArrayList<Company> privateCompanies;

    public final java.util.ArrayList<Company> getCompanies() {
        return privateCompanies;
    }

    private void setCompanies(java.util.ArrayList<Company> value) {
        privateCompanies = value;
    }

    /**
     * All of the passengers.
     */
    private java.util.ArrayList<Passenger> privatePassengers;

    public final java.util.ArrayList<Passenger> getPassengers() {
        return privatePassengers;
    }

    private void setPassengers(java.util.ArrayList<Passenger> value) {
        privatePassengers = value;
    }

    /**
     * Me (my player object).
     */
    private Player privateMe;

    public final Player getMe() {
        return privateMe;
    }

    private void setMe(Player value) {
        privateMe = value;
    }

    private PlayerAIBase.PlayerOrdersEvent sendOrders;

    private static final java.util.Random rand = new java.util.Random();

    public MyPlayerBrain(String name) {
        setName(!net.windward.Windwardopolis.DotNetToJavaStringHelper.isNullOrEmpty(name) ? name : NAME);
    }

    /**
     * The avatar of the player. Must be 32 x 32.
     */
    public final byte[] getAvatar() {
        try {
            // open image
            File file = new File(getClass().getResource("/net/windward/Windwardopolis/res/MyAvatar.png").getFile());

            FileInputStream fisAvatar = new FileInputStream(file);
            byte [] avatar = new byte[fisAvatar.available()];
            fisAvatar.read(avatar, 0, avatar.length);
            return avatar;

        } catch (IOException e) {
            System.out.println("error reading image");
            e.printStackTrace();
            return null;
        }
    }

    /**
     * Called at the start of the game.
     *
     * @param map         The game map.
     * @param me          You. This is also in the players list.
     * @param players     All players (including you).
     * @param companies   The companies on the map.
     * @param passengers  The passengers that need a lift.
     * @param ordersEvent Method to call to send orders to the server.
     */
    public final void Setup(Map map, Player me, java.util.ArrayList<Player> players, java.util.ArrayList<Company> companies, java.util.ArrayList<Passenger> passengers, PlayerAIBase.PlayerOrdersEvent ordersEvent) {

        try {
            setGameMap(map);
            setPlayers(players);
            setMe(me);
            setCompanies(companies);
            setPassengers(passengers);
            sendOrders = ordersEvent;

            java.util.ArrayList<Passenger> pickup = AllPickups(me, passengers);

            // get the path from where we are to the dest.
            java.util.ArrayList<Point> path = CalculatePathPlus1(me, pickup.get(0).getLobby().getBusStop());
            sendOrders.invoke("ready", path, pickup);
        } catch (RuntimeException ex) {
            ex.printStackTrace();
        }
    }

    /**
     * Called to send an update message to this A.I. We do NOT have to send orders in response.
     *
     * @param status     The status message.
     * @param plyrStatus The player this status is about. THIS MAY NOT BE YOU.
     * @param players    The status of all players.
     * @param passengers The status of all passengers.
     */
    public final void GameStatus(PlayerAIBase.STATUS status, Player plyrStatus, java.util.ArrayList<Player> players, java.util.ArrayList<Passenger> passengers) {

        // bugbug - Framework.cs updates the object's in this object's Players, Passengers, and Companies lists. This works fine as long
        // as this app is single threaded. However, if you create worker thread(s) or respond to multiple status messages simultaneously
        // then you need to split these out and synchronize access to the saved list objects.

        try {
            // bugbug - we return if not us because the below code is only for when we need a new path or our limo hit a bus stop.
            // if you want to act on other players arriving at bus stops, you need to remove this. But make sure you use Me, not
            // plyrStatus for the Player you are updatiing (particularly to determine what tile to start your path from).
            Point ptDest = null;
            java.util.ArrayList<Passenger> pickup = new java.util.ArrayList<Passenger>();

            if (plyrStatus != getMe()) {
                switch (status) {
                    case PASSENGER_PICKED_UP:
                        if (plyrStatus.getLimo().getPassenger() == null){
                            pickup = AllPickups(getMe(), passengers);
                            ptDest = pickup.get(0).getLobby().getBusStop();
                        }
                        break;
                    case PASSENGER_DELIVERED:
                    case PASSENGER_ABANDONED:
                        if (plyrStatus.getLimo().getPassenger() == null) {
                            pickup = AllPickups(getMe(), passengers);
                            ptDest = pickup.get(0).getLobby().getBusStop();
                        }
                        break;
                    default:
                        return;
                }
                return;
            } else {
                switch (status) {
                    case UPDATE:
                        return;
                    case NO_PATH:
                    case PASSENGER_NO_ACTION:
                        if (plyrStatus.getLimo().getPassenger() == null) {
                            pickup = AllPickups(plyrStatus, passengers);
                            ptDest = pickup.get(0).getLobby().getBusStop();
                        } else {
                            ptDest = plyrStatus.getLimo().getPassenger().getDestination().getBusStop();
                        }
                        break;
                    case PASSENGER_DELIVERED:
                    case PASSENGER_ABANDONED:
                        pickup = AllPickups(plyrStatus, passengers);
                        ptDest = pickup.get(0).getLobby().getBusStop();
                        break;
                    case PASSENGER_REFUSED:
                        //add in random so no refuse loop
                        for (Company cpy : getCompanies()) {
                            if (cpy != plyrStatus.getLimo().getPassenger().getDestination()) {
                                ptDest = cpy.getBusStop();
                                break;
                            }
                        }
                        break;
                    case PASSENGER_DELIVERED_AND_PICKED_UP:
                    case PASSENGER_PICKED_UP:
                        pickup = AllPickups(plyrStatus, passengers);
                        ptDest = plyrStatus.getLimo().getPassenger().getDestination().getBusStop();
                        break;
                    default:
                        throw new RuntimeException("unknown status");
                }
            }

            // get the path from where we are to the dest.
            java.util.ArrayList<Point> path = CalculatePathPlus1(plyrStatus, ptDest);

            // update our saved Player to match new settings
            if (path.size() > 0) {
                getMe().getLimo().getPath().clear();
                getMe().getLimo().getPath().addAll(path);
            }
            if (pickup.size() > 0) {
                getMe().getPickUp().clear();
                getMe().getPickUp().addAll(pickup);
            }

            sendOrders.invoke("move", path, pickup);
        } catch (RuntimeException ex) {
            ex.printStackTrace();
        }
    }

    private java.util.ArrayList<Point> CalculatePathPlus1(Player me, Point ptDest) {
        java.util.ArrayList<Point> path = SimpleAStar.CalculatePath(getGameMap(), me.getLimo().getMapPosition(), ptDest);
        // add in leaving the bus stop so it has orders while we get the message saying it got there and are deciding what to do next.
        if (path.size() > 1) {
            path.add(path.get(path.size() - 2));
        }
        return path;
    }

    private java.util.ArrayList<Passenger> AllPickups(Player me, Iterable<Passenger> passengers) {
        java.util.ArrayList<Passenger> pickUpOrder = new java.util.ArrayList<Passenger>();

        for (Passenger psngr : passengers) {
            if ((!me.getPassengersDelivered().contains(psngr)) && (psngr != me.getLimo().getPassenger()) && (psngr.getCar() == null) && (psngr.getLobby() != null) && (psngr.getDestination() != null)) {
                pickUpOrder.add(psngr);
            }
        }

        Collections.sort(pickUpOrder, new PassengerComparator(me));

        //add sort by random so no loops for can't pickup
        return pickUpOrder;
    }

    private class PassengerComparator implements Comparator<Passenger> {
        Player me;

        PassengerComparator(Player me) {
            this.me = me;
        }

        public int compare(Passenger p1, Passenger p2) {
            try {
                double p1Cost;
                SimpleAStar.CalculatePath(MyPlayerBrain.this.getGameMap(), me.getLimo().getMapPosition(), p1.getLobby().getBusStop());
                p1Cost = SimpleAStar.last_cost * 0.7;
                SimpleAStar.CalculatePath(MyPlayerBrain.this.getGameMap(), p1.getLobby().getBusStop().getLocation(), p1.getDestination().getBusStop());
                p1Cost += (SimpleAStar.last_cost * 0.3);

                for (Passenger p : p1.getDestination().getPassengers())
                {
                    if (p1.getEnemies().contains(p))
                    {
                        p1Cost += 1000;
                    }
                }

                double p2Cost;
                SimpleAStar.CalculatePath(MyPlayerBrain.this.getGameMap(), me.getLimo().getMapPosition(), p2.getLobby().getBusStop());
                p2Cost = SimpleAStar.last_cost * 0.7;
                SimpleAStar.CalculatePath(MyPlayerBrain.this.getGameMap(), p2.getLobby().getBusStop().getLocation(), p2.getDestination().getBusStop());
                p2Cost += (SimpleAStar.last_cost * 0.3);

                for (Passenger p : p2.getDestination().getPassengers())
                {
                    if (p2.getEnemies().contains(p))
                    {
                        p2Cost += 1000;
                    }
                }

                if (p1.getPointsDelivered() / p1Cost > p2.getPointsDelivered() / p2Cost) {
                    return -1;
                }
                else if (p1.getPointsDelivered() / p1Cost < p2.getPointsDelivered() / p2Cost) {
                    return 1;
                }
                else {
                    int p1DestPassSize = p1.getDestination().getPassengers().size();
                    int p2DestPassSize = p2.getDestination().getPassengers().size();

                    if (p1DestPassSize > p2DestPassSize) {
                        return -1;
                    }
                    else if (p1DestPassSize < p2DestPassSize) {
                        return 1;
                    }
                    else
                    {
                        int randomNum = rand.nextInt(2);
                        return (randomNum == 0) ? -1 : 1;
                    }
                }
            } catch (RuntimeException ex) {
                int p1DestPassSize = p1.getDestination().getPassengers().size();
                int p2DestPassSize = p2.getDestination().getPassengers().size();

                if (p1DestPassSize > p2DestPassSize) {
                    return -1;
                }
                else if (p1DestPassSize < p2DestPassSize) {
                    return 1;
                }
                else
                {
                    int randomNum = rand.nextInt(2);
                    return (randomNum == 0) ? -1 : 1;
                }
            }
        }
    }
}