package com.memoming.busalime.Activity;

import android.content.Intent;
import android.graphics.Color;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.widget.ListView;
import android.widget.TextView;

import com.memoming.busalime.Bus.BusItem;
import com.memoming.busalime.Bus.GetStationListByBusRouteId_Thread;
import com.memoming.busalime.R;
import com.memoming.busalime.Station.Station;
import com.memoming.busalime.Station.StationAdaptor;
import com.memoming.busalime.Station.StationClickListener;

import java.util.ArrayList;

public class StationListActivity extends AppCompatActivity {

    private TextView            busNmTv,backTv,direction;
    private ListView            rltListView;
    private ArrayList<Station>  stationList;
    private Intent              returnIntent;
    private BusItem             clickBusItem;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        try {
            super.onCreate(savedInstanceState);
            setContentView(R.layout.activity_station_list);
            setTitle( "버스 노선도");

            returnIntent    = getIntent();
            clickBusItem    = (BusItem) returnIntent.getSerializableExtra("busItem");
            rltListView     = (ListView) findViewById(R.id.rltList);
            direction       = (TextView) findViewById(R.id.direction);
            direction.setText(clickBusItem.getStStationNm()+"↔"+clickBusItem.getEnStationNm());
            busNmTv         = (TextView) findViewById(R.id.busNm_tv);
            busNmTv.setText(clickBusItem.getBusNm());
            backTv          =(TextView)findViewById(R.id.backTv);

            //간선버스(파란색)
            if(clickBusItem.getBusType().equals("3")){
                backTv.setBackgroundColor(Color.rgb(0,102,204));
                busNmTv.setTextColor(Color.WHITE);
                direction.setTextColor(Color.WHITE);
            }
            //지선버스(녹색)
            else if(clickBusItem.getBusType().equals("4")){
                backTv.setBackgroundColor(Color.rgb(0,153,51));
                busNmTv.setTextColor(Color.WHITE);
                direction.setTextColor(Color.WHITE);
            }
            //순환버스 (개나리색)
            else if(clickBusItem.getBusType().equals("5")){
                backTv.setBackgroundColor(Color.rgb(255,204,0));
                busNmTv.setTextColor(Color.WHITE);
                direction.setTextColor(Color.WHITE);
            }
            //광역버스 (빨간색)
            else {
                backTv.setBackgroundColor(Color.rgb(255,51,000));
                busNmTv.setTextColor(Color.WHITE);
                direction.setTextColor(Color.WHITE);
            }

            GetStationListByBusRouteId_Thread thread = new GetStationListByBusRouteId_Thread(clickBusItem.getRouteId());
            thread.start();
            thread.join();
            stationList = thread.getStationList();

            StationAdaptor stationAdaptor = new StationAdaptor(getBaseContext(), stationList);
            rltListView.setAdapter(stationAdaptor);

            // 리스트뷰에 클릭 리스너 달기
            StationClickListener stationClickListener = new StationClickListener();
            rltListView.setOnItemClickListener(stationClickListener);


        }
        catch (InterruptedException e) { e.printStackTrace(); }
    } // end of onCreate

} // end of class
