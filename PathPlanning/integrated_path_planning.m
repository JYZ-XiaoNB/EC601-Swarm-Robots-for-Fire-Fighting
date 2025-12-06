function integrated_path_planning
    % %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    % %%  INTEGRATED XAI DASHBOARD & PATH PLANNING (FIXED)
    % %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    %
    % This app demonstrates "Explainable AI" behavior.
    % It visualizes the robot identifying a blocked path,
    % explaining why it is blocked, and calculating a new route.
    %

    % --- 1. Define Map Data ---
    start_pt = [5, 5];
    end_pt   = [45, 40];
    
    % The obstacles that blocked us before
    obs1 = [10, 15];
    obs2 = [30, 25]; 
    obstacles = [obs1; obs2];

    % --- 2. Create the App Layout ---
    fig = uifigure('Name', 'XAI Path Planning Dashboard', ...
                   'Position', [100, 100, 900, 500]);

    % Use a Grid Layout: Plot on Left, Dashboard on Right
    g = uigridlayout(fig, [1, 2]);
    g.ColumnWidth = {'2x', '1x'}; % Plot takes 2/3rds of the width

    % -- Left Panel: The Plot --
    ax = uiaxes(g);
    ax.Title.String = 'Real-Time Trajectory Map';
    ax.XLabel.String = 'X Coordinate';
    ax.YLabel.String = 'Y Coordinate';
    ax.Layout.Column = 1;
    grid(ax, 'on');
    axis(ax, 'equal');
    
    % -- Right Panel: The Dashboard --
    % We use a sub-grid for the dashboard controls
    dashGrid = uigridlayout(g, [3, 1]);
    dashGrid.Layout.Column = 2;
    dashGrid.RowHeight = {'fit', '1x', 'fit'};

    % Label
    uilabel(dashGrid, 'Text', 'XAI Decision Log:', 'FontWeight', 'bold');

    % Text Area for the Log
    logArea = uitextarea(dashGrid, 'Editable', 'off');
    
    % "Run Simulation" Button
    runBtn = uibutton(dashGrid, 'Text', 'RUN SIMULATION', ...
                      'BackgroundColor', [0.3, 0.6, 1], ...
                      'FontWeight', 'bold', ...
                      'FontColor', 'white');
    
    % --- 3. The Simulation Logic ---
    % When the button is pressed, this function runs
    runBtn.ButtonPushedFcn = @(btn, event) runSimulation();

    function runSimulation()
        % Reset the view
        cla(ax); % Clear axes
        
        % --- FIX IS HERE ---
        % We use cell(0,1) to create an empty N-by-1 array
        logArea.Value = cell(0,1); 
        % -------------------
        
        disableControl(runBtn); % Prevent double-clicking
        
        % -- Step 1: Initialize Map --
        updateLog('SYSTEM: Initializing sensor map...');
        hold(ax, 'on');
        
        % Plot Start/End
        plot(ax, start_pt(1), start_pt(2), 'go', 'MarkerSize', 12, 'MarkerFaceColor', 'g', 'DisplayName', 'Start');
        plot(ax, end_pt(1), end_pt(2), 'rx', 'MarkerSize', 12, 'LineWidth', 2, 'DisplayName', 'Target');
        
        % Plot Obstacles
        plot(ax, obstacles(:,1), obstacles(:,2), 'ks', 'MarkerSize', 12, 'MarkerFaceColor', 'r', 'DisplayName', 'Obstacles');
        legend(ax, 'Location', 'northwest');
        pause(1.0);

        % -- Step 2: Calculate Direct Path (The Failure) --
        updateLog('PLANNER: Calculating optimal direct path...');
        plot(ax, [start_pt(1), end_pt(1)], [start_pt(2), end_pt(2)], 'b--', 'LineWidth', 1.5, 'DisplayName', 'Original Path');
        pause(1.0);
        
        updateLog('ALERT: Path obstructed! Collision detected at [10,15] and [30,25].');
        updateLog('XAI: Direct route unsafe. Initiating obstacle avoidance algorithm.');
        pause(1.5);

        % -- Step 3: Execute Avoidance (Segment by Segment) --
        
        % Waypoint 1: Go UNDER Obstacle 1 (10, 15)
        % We target (15, 10) to clear it safely.
        wp1 = [15, 10]; 
        
        updateLog('DECISION: Rerouting to (15, 10) to avoid Obstacle 1.');
        animatePath(start_pt, wp1);
        pause(1.0);
        
        % Waypoint 2: Go UNDER Obstacle 2 (30, 25)
        % Previous code went straight to (35, 30), which hit the obstacle.
        % NEW TARGET: (35, 20). This dips "below" the obstacle at Y=25.
        wp2 = [35, 20]; 
        
        updateLog('DECISION: Detected Obstacle 2 at (30, 25). Adjusting course to (35, 20) to maintain safety margin.');
        animatePath(wp1, wp2);
        pause(1.0);
        
        % Waypoint 3: Proceed to End
        updateLog('PLANNER: Path to target is clear. Proceeding.');
        animatePath(wp2, end_pt);
        
        updateLog('SUCCESS: Target reached safely.');
        
        % Re-enable button
        runBtn.Text = 'RESET & RUN AGAIN';
        runBtn.Enable = 'on';
    end

    % -- Helper: Animate drawing a line --
    function animatePath(p1, p2)
        % Draws a green line from p1 to p2
        plot(ax, [p1(1), p2(1)], [p1(2), p2(2)], 'g-', 'LineWidth', 3);
    end

    % -- Helper: Update the text log --
    function updateLog(msg)
        timestamp = datestr(now, 'HH:MM:SS');
        newEntry = sprintf('[%s] %s', timestamp, msg);
        currentLog = logArea.Value;
        
        % Ensure we are appending to a valid list
        if isempty(currentLog)
             logArea.Value = {newEntry};
        else
             logArea.Value = [currentLog; {newEntry}];
        end
        
        scroll(logArea, 'bottom');
    end

    % -- Helper: Disable button during run --
    function disableControl(btn)
        btn.Enable = 'off';
        btn.Text = 'RUNNING...';
    end
end