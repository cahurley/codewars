package net.windward.Windwardopolis.api;


import java.awt.*;
import org.dom4j.Element;
public class Company
{
	private Company(Element elemCompany)
	{
		setName(elemCompany.attributeValue("name"));
		setBusStop(new Point(Integer.parseInt(elemCompany.attributeValue("bus-stop-x")), Integer.parseInt(elemCompany.attributeValue("bus-stop-y"))));
		setPassengers(new java.util.ArrayList<Passenger>());
	}

	/** 
	 The name of the company.
	*/
	private String privateName;
	public final String getName()
	{
		return privateName;
	}
	private void setName(String value)
	{
		privateName = value;
	}

	/** 
	 The tile with the company's bus stop.
	*/
	private Point privateBusStop;
	public final Point getBusStop()
	{
		return privateBusStop;
	}
	private void setBusStop(Point value)
	{
		privateBusStop = value;
	}

	/** 
	 The name of the passengers waiting at this company's bus stop for a ride.
	*/
	private java.util.List<Passenger> privatePassengers;
	public final java.util.List<Passenger> getPassengers()
	{
		return privatePassengers;
	}
	private void setPassengers(java.util.List<Passenger> value)
	{
		privatePassengers = value;
	}

	public static java.util.ArrayList<Company> FromXml(Element elemCompanies)
	{
		java.util.ArrayList<Company> companies = new java.util.ArrayList<Company>();
		for (Object objCmpyOn : elemCompanies.selectNodes("company"))
		{
            Element elemCmpyOn = (Element) objCmpyOn;
			companies.add(new Company(elemCmpyOn));
		}
		return companies;
	}

	@Override
	public String toString()
	{
		return String.format("%1$s; %2$s", getName(), getBusStop());
	}
}