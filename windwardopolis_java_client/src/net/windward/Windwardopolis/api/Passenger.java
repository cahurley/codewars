package net.windward.Windwardopolis.api;

import net.windward.Windwardopolis.TRAP;
import org.dom4j.Element;
import org.dom4j.Attribute;

public class Passenger {
    private Passenger(Element elemPassenger, java.util.ArrayList<Company> companies) {
        setName(elemPassenger.attribute("name").getValue());
        setPointsDelivered(Integer.parseInt(elemPassenger.attribute("points-delivered").getValue()));
        Attribute attr = elemPassenger.attribute("lobby");
        if (attr != null) {
            for(Company cpy : companies)
            {
                if(cpy.getName().equalsIgnoreCase(attr.getValue()))
                    setLobby(cpy);
            }
        }
        attr = elemPassenger.attribute("destination");
        if (attr != null) {
            for(Company cpy : companies)
            {
                if(cpy.getName().equalsIgnoreCase(attr.getValue()))
                    setDestination(cpy);
            }
        }
        setRoute(new java.util.ArrayList<Company>());
        for (Object objRoute : elemPassenger.selectNodes("route")) {
            Element elemRoute = (Element)objRoute;
            for(Company cpy : companies)
            {
                if(cpy.getName().equalsIgnoreCase(elemRoute.getText()))
                {
                    getRoute().add(cpy);
                }
            }
        }
        setEnemies(new java.util.ArrayList<Passenger>());
    }

    /**
     * The name of this passenger.
     */
    private String privateName;

    public final String getName() {
        return privateName;
    }

    private void setName(String value) {
        privateName = value;
    }

    /**
     * The number of points a player gets for delivering this passenger.
     */
    private int privatePointsDelivered;

    public final int getPointsDelivered() {
        return privatePointsDelivered;
    }

    private void setPointsDelivered(int value) {
        privatePointsDelivered = value;
    }

    /**
     * The limo the passenger is presently in. null if not in a limo.
     */
    private Limo privateCar;

    public final Limo getCar() {
        return privateCar;
    }

    public final void setCar(Limo value) {
        privateCar = value;
    }

    /**
     * The bus stop the passenger is presently waiting in. null if in a limo or has arrived at final destination.
     */
    private Company privateLobby;

    public final Company getLobby() {
        return privateLobby;
    }

    private void setLobby(Company value) {
        privateLobby = value;
    }

    /**
     * The company the passenger wishes to go to. This is valid both at a bus stop and in a car. It is null if
     * they have been delivered to their final destination.
     */
    private Company privateDestination;

    public final Company getDestination() {
        return privateDestination;
    }

    private void setDestination(Company value) {
        privateDestination = value;
    }

    /**
     * The remaining companies the passenger wishes to go to after destination, in order. This does not include
     * the Destination company.
     */
    private java.util.List<Company> privateRoute;

    public final java.util.List<Company> getRoute() {
        return privateRoute;
    }

    private void setRoute(java.util.List<Company> value) {
        privateRoute = value;
    }

    /**
     * If any of these passengers are at a bus stop, this passenger will not exit the car at the bus stop.
     * If a passenger at the bus stop has this passenger as an enemy, the passenger can still exit the car.
     */
    private java.util.List<Passenger> privateEnemies;

    public final java.util.List<Passenger> getEnemies() {
        return privateEnemies;
    }

    private void setEnemies(java.util.List<Passenger> value) {
        privateEnemies = value;
    }

    public static java.util.ArrayList<Passenger> FromXml(Element elemPassengers, java.util.ArrayList<Company> companies) {
        java.util.ArrayList<Passenger> passengers = new java.util.ArrayList<Passenger>();
        for (Object elemPsngrOn : elemPassengers.selectNodes("passenger")) {
            passengers.add(new Passenger((Element) elemPsngrOn, companies));
        }

        // need to now assign enemies - needed all Passenger objects created first.
        for (Object objPsngrOn : elemPassengers.selectNodes("passenger")) {
            Element elemPsngrOn = (Element) objPsngrOn;
            Passenger psngrOn = null;
            for(Passenger psngr : passengers)
            {
                if(psngr.getName().equalsIgnoreCase(elemPsngrOn.attribute("name").getValue()))
                {
                    psngrOn = psngr;
                }
            }


            for (Object objEnemyOn : elemPsngrOn.selectNodes("enemy")) {
                Element elemEnemyOn = (Element) objEnemyOn;
                for(Passenger psngr : passengers)
                {

                    if( psngr.getName().equalsIgnoreCase(elemEnemyOn.getText()) && psngrOn!=null)
                        psngrOn.getEnemies().add(psngr);
                }
            }
        }

        // set if they're in a lobby
        for (Passenger psngrOn : passengers) {
            if (psngrOn.getLobby() == null) {
                continue;
            }
            Company cmpnyOn=null;
            for(Company cmpny : companies)
            {
                if(cmpny == psngrOn.getLobby())
                    cmpnyOn = cmpny;
            }
            if (cmpnyOn != null) {
                cmpnyOn.getPassengers().add(psngrOn);
            }
        }

        return passengers;
    }

    public static void UpdateFromXml(java.util.ArrayList<Passenger> passengers, java.util.ArrayList<Company> companies, Element elemPassengers) {
        for (Object objPsngrOn : elemPassengers.selectNodes("passenger")) {
            Element elemPsngrOn = (Element) objPsngrOn;
            Passenger psngrOn=null;

            for (Passenger ps : passengers) {
                if (ps.getName().equalsIgnoreCase(elemPsngrOn.attribute("name").getValue()))
                    psngrOn = ps;
            }

            Attribute attr = elemPsngrOn.attribute("destination");
            if (attr != null) {
                for (Company cmpy : companies) {
                    if(cmpy.getName().equalsIgnoreCase(attr.getValue()) && psngrOn!=null)
                        psngrOn.setDestination(cmpy);
                }

                // remove from the route
                if (psngrOn.getRoute().contains(psngrOn.getDestination())) {
                    psngrOn.getRoute().remove(psngrOn.getDestination());
                }
            }

            // set props based on waiting, travelling, done
            if (elemPsngrOn.attribute("status").getValue().equals("lobby")) {
                for (Company cmpy : companies) {
                    if (cmpy.getName().equalsIgnoreCase(elemPsngrOn.attribute("lobby").getValue()))
                        psngrOn.setLobby(cmpy);
                }
                psngrOn.setCar(null);
            } else if (elemPsngrOn.attribute("status").getValue().equals("travelling")) {
                psngrOn.setLobby(null);
                // psngrOn.Car set in Player update.
            } else if (elemPsngrOn.attribute("status").getValue().equals("done")) {
                TRAP.trap();
                psngrOn.setDestination(null);
                psngrOn.setLobby(null);
                psngrOn.setCar(null);
            }
        }
    }

    @Override
    public String toString() {
        return getName();
    }
}