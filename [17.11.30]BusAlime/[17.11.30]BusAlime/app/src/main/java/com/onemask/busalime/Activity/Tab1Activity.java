package com.onemask.busalime.Activity;

import android.graphics.Color;
import android.os.Bundle;
import android.support.v7.app.ActionBarActivity;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ListView;
import android.widget.TextView;

import com.onemask.busalime.Station.GetStationByName_Thread;
import com.onemask.busalime.Station.StationAdaptor;
import com.onemask.busalime.Station.StationClickListener;
import com.soo.busalime.R;

import java.io.UnsupportedEncodingException;

/**
 * Created by Soo on 2017-09-12.
 */

public class Tab1Activity extends ActionBarActivity {


    private TextView        tvw, textView2;
    private EditText        input_stNm;
    private Button          btn;
    private ListView        rltList;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_tab1);
        setTitle("정류소 검색");

        textView2 = (TextView) findViewById(R.id.textView2) ;
        textView2.setBackgroundColor(Color.LTGRAY);

        tvw = (TextView) findViewById(R.id.textView);
        input_stNm = (EditText) findViewById(R.id.stName);
        input_stNm.setBackgroundColor(Color.WHITE);

        btn = (Button) findViewById(R.id.button3);
        rltList = (ListView) findViewById(R.id.rltList);

        btn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                stationExec();
            }
        });
    }


    private void stationExec () {
        try {
            GetStationByName_Thread thread
                    = new GetStationByName_Thread( input_stNm.getText().toString() );
            thread.start();
            thread.join();

            rltList.setAdapter ( new StationAdaptor(getBaseContext(), thread.getStations()) );
            rltList.setOnItemClickListener ( new StationClickListener() );
        }
        catch (InterruptedException e)          { e.printStackTrace(); }
        catch (UnsupportedEncodingException e)  { e.printStackTrace(); }
    } // end of stationExec


}
