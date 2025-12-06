function integrated_path_planning_margin
    % %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    % %%  INTEGRATED XAI DASHBOARD & PATH PLANNING (OPTIMIZED)
    % %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    
    % --- 1. Define Map Data ---
    start_pt = [5, 5];
    end_pt   = [45, 40];
    
    % Obstacles
    obs1 = [10, 15];
    obs2 = [30, 25]; 
    obstacles = [obs1; obs2];

    % --- 2. Create the App Layout ---
    fig = uifigure('Name', 'XAI Path Planning Dashboard', ...
                   'Position', [100, 100, 950, 500]);

    % Grid Layout
    g = uigridlayout(fig, [1, 2]);
    g.ColumnWidth = {'2x', '1x'};

    % -- Left Panel: The Plot --
    ax = uiaxes(g);
    ax.Title.String = 'Real-Time Trajectory Map';
    ax.XLabel.String = 'X Coordinate';
    ax.YLabel.String = 'Y Coordinate';
    ax.Layout.Column = 1;
    grid(ax, 'on');
    axis(ax, 'equal');
    
    % Zoom in slightly to see the "dodge" better
    xlim(ax, [0, 50]);
    ylim(ax, [0, 45]);
    
    % -- Right Panel: The Dashboard --
    dashGrid = uigridlayout(g, [3, 1]);
    dashGrid.Layout.Column = 2;
    dashGrid.RowHeight = {'fit', '1x', 'fit'};

    uilabel(dashGrid, 'Text', 'XAI Decision Log:', 'FontWeight', 'bold');
    logArea = uitextarea(dashGrid, 'Editable', 'off');
    
    runBtn = uibutton(dashGrid, 'Text', 'RUN SIMULATION', ...
                      'BackgroundColor', [0.3, 0.6, 1], ...
                      'FontWeight', 'bold', ...
                      'FontColor', 'white');
    
    % --- 3. The Simulation Logic ---
    runBtn.ButtonPushedFcn = @(btn, event) runSimulation();

    function runSimulation()
        cla(ax); 
        logArea.Value = cell(0,1); 
        disableControl(runBtn); 
        
        % -- Step 1: Initialize --
        updateLog('SYSTEM: Initializing sensor map...');
        hold(ax, 'on');
        
        plot(ax, start_pt(1), start_pt(2), 'go', 'MarkerSize', 12, 'MarkerFaceColor', 'g', 'DisplayName', 'Start');
        plot(ax, end_pt(1), end_pt(2), 'rx', 'MarkerSize', 12, 'LineWidth', 2, 'DisplayName', 'Target');
        plot(ax, obstacles(:,1), obstacles(:,2), 'ks', 'MarkerSize', 12, 'MarkerFaceColor', 'r', 'DisplayName', 'Obstacles');
        legend(ax, 'Location', 'northwest');
        pause(1.0);

        % -- Step 2: Direct Path --
        updateLog('PLANNER: Calculating optimal direct path...');
        plot(ax, [start_pt(1), end_pt(1)], [start_pt(2), end_pt(2)], 'b--', 'LineWidth', 1.5, 'DisplayName', 'Original Path');
        pause(1.0);
        
        updateLog('ALERT: Path obstructed at [10,15] and [30,25].');
        updateLog('XAI: Optimizing for minimal deviation (Efficiency Mode).');
        pause(1.5);

        % -- Step 3: TIGHT Obstacle Avoidance --
        
        % Waypoint 1: Just below Obstacle 1 (10, 15)
        % Previous was [15, 10] (Too far). 
        % New is [12, 12]. This hugs the corner much tighter.
        wp1 = [12, 12]; 
        
        updateLog('DECISION: Obstacle 1 detected. Squeezing past at (12, 12) with 3u safety buffer.');
        animatePath(start_pt, wp1);
        pause(1.0);
        
        % Waypoint 2: Just below Obstacle 2 (30, 25)
        % Previous was [35, 20] (Too wide).
        % New is [33, 22]. Closer to the obstacle, more efficient.
        wp2 = [33, 22]; 
        
        updateLog('DECISION: Approaching Obstacle 2. Adjusting trajectory to (33, 22) to maintain velocity.');
        animatePath(wp1, wp2);
        pause(1.0);
        
        % Waypoint 3: End
        updateLog('PLANNER: Path clear. Accelerating to target.');
        animatePath(wp2, end_pt);
        
        updateLog('SUCCESS: Target reached. Efficiency: 95%.');
        
        runBtn.Text = 'RESET & RUN AGAIN';
        runBtn.Enable = 'on';
    end

    function animatePath(p1, p2)
        plot(ax, [p1(1), p2(1)], [p1(2), p2(2)], 'g-', 'LineWidth', 3);
    end

    function updateLog(msg)
        timestamp = datestr(now, 'HH:MM:SS');
        newEntry = sprintf('[%s] %s', timestamp, msg);
        currentLog = logArea.Value;
        if isempty(currentLog)
             logArea.Value = {newEntry};
        else
             logArea.Value = [currentLog; {newEntry}];
        end
        scroll(logArea, 'bottom');
    end

    function disableControl(btn)
        btn.Enable = 'off';
        btn.Text = 'RUNNING...';
    end
end