package com.onemask.busalime.Bus;

import com.onemask.busalime.Station.Station;
import com.onemask.busalime.Util.DataGoKr;

import org.xmlpull.v1.XmlPullParser;
import org.xmlpull.v1.XmlPullParserException;
import org.xmlpull.v1.XmlPullParserFactory;

import java.io.IOException;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.ArrayList;

public class GetStationListByBusRouteId_Thread extends Thread {

    private DataGoKr dataGoKr;
    private Station stationItem;
    private ArrayList<Station> stationList;
    private String requestURL;
    private URL url;

    private int eventType;
    private XmlPullParserFactory factory;
    private XmlPullParser parser;


    public GetStationListByBusRouteId_Thread (String routeId) {
        dataGoKr = new DataGoKr();
        requestURL = dataGoKr.getUrl_getStationList_ByBusRouteId()
                + "&busRouteId="
                + routeId;
    }

    @Override
    public void run() {
        try {
            stationItem = null;
            stationList = new ArrayList<Station>();
            factory     = XmlPullParserFactory.newInstance();
            parser      = factory.newPullParser();
            url         = new URL( requestURL );
            parser.setInput(url.openStream(), "UTF-8");
            eventType   = parser.getEventType();

            while (eventType != XmlPullParser.END_DOCUMENT) {
                switch (eventType) {
                    case XmlPullParser.START_TAG:
                        String tag = parser.getName();

                        if      (tag.compareTo("itemList") == 0 && stationItem == null)
                            stationItem = new Station();
                        else if (tag.compareTo("arsId") == 0)       stationItem.setArsId(parser.nextText());
                        else if (tag.compareTo("direction") == 0)   stationItem.setDirection(parser.nextText());
                        else if (tag.compareTo("gpsX") == 0)        stationItem.setGpsY(parser.nextText());
                        else if (tag.compareTo("gpsY") == 0)        stationItem.setGpsX(parser.nextText());
                        else if (tag.compareTo("stationNm") == 0)   stationItem.setStNm(parser.nextText());
                        else if (tag.compareTo("transYn") == 0) {
                            stationItem.setTrans(parser.nextText());
                            stationList.add(stationItem);
                            stationItem = null;
                        }

                        break;
                } // end of switch
                eventType = parser.next();
            } // end of while
        } // end of try

        catch (XmlPullParserException e)    { e.printStackTrace(); }
        catch (MalformedURLException e)     { e.printStackTrace(); }
        catch (IOException e)               { e.printStackTrace(); }

    } // end of run

    public ArrayList<Station> getStationList () { return stationList; }

} // end of class
