package com.memoming.busalime.Activity;

import android.graphics.Color;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ListView;
import android.widget.TextView;

import com.memoming.busalime.Bus.BusAdaptor;
import com.memoming.busalime.Bus.BusClickListener;
import com.memoming.busalime.Bus.GetBusListByBusNumber_Thread;
import com.memoming.busalime.R;

/**
 * Created by Soo on 2017-09-12.
 */

public class Tab2Activity  extends AppCompatActivity{

    private TextView textView,textView2;
    private EditText stName;
    private Button button3;
    private ListView rltList;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_tab2);

        textView = (TextView) findViewById(R.id.textView);
        textView2 = (TextView)findViewById(R.id.textView2);
        textView2.setBackgroundColor(Color.LTGRAY);

        stName = (EditText) findViewById(R.id.stName);
        button3 = (Button) findViewById(R.id.button3);
        rltList = (ListView) findViewById(R.id.rltList);


        button3.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                busNumberExec();
            }
        });
    }



    private void busNumberExec () {
        try {
            GetBusListByBusNumber_Thread thread
                    = new GetBusListByBusNumber_Thread( stName.getText().toString() );
            thread.start();
            thread.join();

            rltList.setAdapter( new BusAdaptor(getBaseContext(), thread.getBusList()) );
            rltList.setOnItemClickListener( new BusClickListener() );
        }
        catch (InterruptedException e) { e.printStackTrace(); }
    } // end of busNumberExec


}


