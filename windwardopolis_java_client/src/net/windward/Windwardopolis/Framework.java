package net.windward.Windwardopolis;

import net.windward.Windwardopolis.AI.MyPlayerBrain;
import net.windward.Windwardopolis.AI.PlayerAIBase;
import net.windward.Windwardopolis.api.Company;
import net.windward.Windwardopolis.api.Map;
import net.windward.Windwardopolis.api.Passenger;
import net.windward.Windwardopolis.api.Player;
import org.dom4j.*;
import org.dom4j.io.SAXReader;
import sun.misc.BASE64Encoder;

import java.io.*;
import java.util.ArrayList;
import java.util.Arrays;
import java.awt.*;
// Created by Windward Studios, Inc. (www.windward.net). No copyright claimed - do anything you want with this code.


public class Framework implements IPlayerCallback {
    private TcpClient tcpClient;
    private MyPlayerBrain brain;
    private String ipAddress = "127.0.0.1";

    private String myGuid;

    // this is used to make sure we don't have multiple threads updating the Player/Passenger lists, sending
    // back multiple orders, etc. This is a lousy way to handle this - but it keeps the example simple and
    // leaves room for easy improvement.
    private int signal;

    //private static final log4net.ILog log = log4net.LogManager.GetLogger(Framework.class);

    /**
     * Run the A.I. player. All parameters are optional.
     *
     * @param args I.P. address of server, name
     */
    public static void main(String[] args) throws IOException {
        Framework framework = new Framework(Arrays.asList(args));
        framework.Run();
    }

    private Framework(java.util.List<String> args) {
        brain = new MyPlayerBrain(args.size() >= 2 ? args.get(1) : null);
        if (args.size() >= 1) {
            ipAddress = args.get(0);
        }
        String msg = String.format("Connecting to server %1$s for user: %2$s", ipAddress, brain.getName());

        System.out.println(msg);
    }

    private void Run() throws IOException {
        System.out.println("starting...");

        tcpClient = new TcpClient(this, ipAddress);
        tcpClient.Start();
        ConnectToServer();

        // It's all messages to us now.
        System.out.println("enter \"exit\" to exit program");
        while (true) {
            BufferedReader in = new BufferedReader(new InputStreamReader(System.in));
            String line = in.readLine();
            if (line.equals("exit")) {
                System.out.println("Exiting program...");
                tcpClient.abort();
                break;
            }
        }
    }

    public final void StatusMessage(String message) {
        System.out.println(message);
    }

    public final void IncomingMessage(String message) throws DocumentException {
        try {
            long startTime = System.currentTimeMillis();
            // get the xml - we assume we always get a valid message from the server.
            SAXReader reader = new SAXReader();
            Document xml = reader.read(new StringReader(message));

            String rootName = xml.getRootElement().getName();

            if (rootName.equals("setup")) {
                System.out.println("Received setup message");

                java.util.ArrayList<Player> players = Player.FromXml(xml.getRootElement().element("players"));
                java.util.ArrayList<Company> companies = Company.FromXml(xml.getRootElement().element("companies"));
                java.util.ArrayList<Passenger> passengers = Passenger.FromXml(xml.getRootElement().element("passengers"), companies);
                Map map = new Map(xml.getRootElement().element("map"), companies);
                myGuid = xml.getRootElement().attribute("my-guid").getValue();

                Player me2 = null;
                for(Player plyr : players)
                {
                    if (myGuid.equals(plyr.getGuid()))
                        me2 = plyr;
                }

                brain.Setup(map, me2, players, companies, passengers, new PlayerAIBase.PlayerOrdersEvent() {
                    public void invoke(String order, ArrayList<Point> path, ArrayList<Passenger> pickUp) {
                        PlayerOrdersEvent(order, path, pickUp);
                    }
                });


            }
//ORIGINAL LINE: case "status":
            else if (rootName.equals("status")) {
                // may be here because re-started and got this message before the re-send of setup.
                if (net.windward.Windwardopolis.DotNetToJavaStringHelper.isNullOrEmpty(myGuid)) {
                    TRAP.trap();
                    return;
                }

                PlayerAIBase.STATUS status = PlayerAIBase.STATUS.valueOf(xml.getRootElement().attribute("status").getValue());
                Attribute attr = xml.getRootElement().attribute("player-guid");
                String guid = attr != null ? attr.getValue() : myGuid;

                synchronized (this) {
                    if (signal > 0) {
                        // bad news - we're throwing this message away.
                        TRAP.trap();
                        return;
                    }
                    signal++;
                }

                Player.UpdateFromXml(brain.getPlayers(), brain.getPassengers(),xml.getRootElement().element("players"));
                Passenger.UpdateFromXml(brain.getPassengers(), brain.getCompanies(), xml.getRootElement().element("passengers"));


                // update my path & pick-up.
                Player plyrStatus = null;
                for(Player plyr :brain.getPlayers())
                {
                    if(guid.equals(plyr.getGuid()))
                        plyrStatus = plyr;
                }
                Element elem = xml.getRootElement().element("path");
                if (elem != null) {
                    String[] path = elem.getText().split(";", 0);
                    plyrStatus.getLimo().getPath().clear();
                    for (String stepOn : path) {
                        int pos = stepOn.indexOf(',');
                        if(pos>0)
                        plyrStatus.getLimo().getPath().add(new Point(Integer.parseInt(stepOn.substring(0, pos)), Integer.parseInt(stepOn.substring(0, pos))));
                    }
                }

                elem = xml.getRootElement().element("pick-up");
                if (elem != null) {
                    String[] names = elem.getText().split(";", 0);
                    plyrStatus.getPickUp().clear();

                    ArrayList<Passenger> newPsngrList = new ArrayList<Passenger>();

                    for(String name : names)
                    {
                        for(Passenger ps : brain.getPassengers())
                        {
                            if(ps.getName().equals(name))
                            {
                                newPsngrList.add(ps);
                            }
                        }

                    }

                    for (Passenger psngrOn : newPsngrList)
                    {
                        plyrStatus.getPickUp().add(psngrOn);
                    }
                }

                // pass in to generate new orders
                brain.GameStatus(status, plyrStatus, brain.getPlayers(), brain.getPassengers());

                synchronized (this) {
                    signal--;
                }

            }
//ORIGINAL LINE: case "exit":
            else if (xml.getRootElement().getName().equals("exit")) {
                System.out.println("Received exit message");
               /* if (log.getIsInfoEnabled()) {
                    log.Info("Received exit message");
                } */
                System.exit(0);

            } else {
                TRAP.trap();
               // String msg = String.format("ERROR: bad message (XML) from server - root node %1$s", xml.Root.Name.LocalName);
               // log.Warn(msg);
               // Trace.WriteLine(msg);
            }

            long turnTime = System.currentTimeMillis() - startTime;
            if (turnTime > 800) {
                System.out.println("WARNING - turn took " + turnTime / 1000 + " seconds");

            }
        } catch (RuntimeException ex) {
            System.out.println(String.format("Error on incoming message. Exception: %1$s", ex));
            ex.printStackTrace();
            //log.Error("Error on incoming message.", ex);
        }
    }

    private void PlayerOrdersEvent(String order, java.util.ArrayList<Point> path, java.util.ArrayList<Passenger> pickUp) {

        // update our info
        if (path.size() > 0) {
            brain.getMe().getLimo().getPath().clear();
            brain.getMe().getLimo().getPath().addAll(path);
        }
        if (pickUp.size() > 0) {
            brain.getMe().getPickUp().clear();
            brain.getMe().getPickUp().addAll(pickUp);
        }
        Document xml = DocumentHelper.createDocument();
        Element elem = DocumentHelper.createElement(order);
        xml.add(elem);
        if (path.size() > 0) {
            StringBuilder buf = new StringBuilder();
            for (Point ptOn : path) {
                buf.append(String.valueOf(ptOn.x) + ',' + String.valueOf(ptOn.y) + ';');
            }
            Element newElem = DocumentHelper.createElement("path");
            newElem.setText(buf.toString());
            elem.add(newElem);
        }
        if (pickUp.size() > 0) {
            StringBuilder buf = new StringBuilder();
            for (Passenger psngrOn : pickUp) {
                buf.append(psngrOn.getName() + ';');
            }
            Element newElem = DocumentHelper.createElement("pick-up");
            newElem.setText(buf.toString());
            elem.add(newElem);
        }
        try {
            tcpClient.SendMessage(xml.asXML());
        } catch (IOException e) {
            System.out.println("bad sent orders event");
            e.printStackTrace();
        }
    }

    public final void ConnectionLost(Exception ex) throws IOException, InterruptedException {

        System.out.println("Lost our connection! Exception: " + ex.getMessage());

        int delay = 500;
        while (true) {
            try {
                if (tcpClient != null) {
                    tcpClient.Close();
                }
                tcpClient = new TcpClient(this,ipAddress);
                tcpClient.Start();

                ConnectToServer();
                System.out.println("Re-connected");

                return;
            } catch (RuntimeException e) {

                System.out.println("Re-connection fails! Exception: " + e.getMessage());
                Thread.sleep(delay);
                delay += 500;
            }
        }
    }

    private void ConnectToServer() throws IOException {
        Document doc = DocumentHelper.createDocument();
        Element root = DocumentHelper.createElement("join");
	    root.addAttribute("name",brain.getName());
	    root.addAttribute("school",MyPlayerBrain.SCHOOL);
	    root.addAttribute("language","Java");

        //TODO add avatar

        byte[] data = brain.getAvatar();
        if (data != null) {
            Element avatarElement = DocumentHelper.createElement("avatar");
            BASE64Encoder encoder = new BASE64Encoder();
            avatarElement.setText(encoder.encode(data));
          	root.add(avatarElement);
        }

        doc.add(root);

        tcpClient.SendMessage(doc.asXML());
    }
}