import React, { useState, useRef } from 'react';
import {
  View,
  Text,
  TouchableOpacity,
  Switch,
  Platform,
  StyleSheet,
  ScrollView,
  Alert,
  SafeAreaView,
} from 'react-native';
import { Picker } from '@react-native-picker/picker';
import Svg, {
  Path,
  Rect,
  Circle,
  G,
  Text as SvgText,
  Defs,
} from 'react-native-svg';
// Import with platform checks to avoid native module errors
const ViewShot = Platform.OS === 'web' ? null : require('react-native-view-shot').default;
const FileSystem = Platform.OS === 'web' ? null : require('react-native-fs');
const Sharing = Platform.OS === 'web' ? null : require('expo-sharing');
import { CopilotStep, walkthroughable, CopilotProvider } from 'react-native-copilot';

// For web compatibility
const WebColorPicker = Platform.OS === 'web' ? require('./WebColorPicker').default : null;
const WalkthroughableView = walkthroughable(View);

// Styles
const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: 16,
    backgroundColor: '#fff',
  },
  scrollView: {
    flex: 1,
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 16,
  },
  subtitle: {
    fontSize: 18,
    fontWeight: 'bold',
    marginVertical: 12,
  },
  colorPickerContainer: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    justifyContent: 'space-between',
    marginBottom: 16,
  },
  colorPickerItem: {
    width: '48%',
    marginBottom: 12,
  },
  colorLabel: {
    marginBottom: 8,
    fontWeight: '500',
  },
  colorValue: {
    marginTop: 8,
  },
  tilesGrid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    justifyContent: 'space-around',
    marginBottom: 24,
  },
  tileContainer: {
    alignItems: 'center',
    width: Platform.OS === 'web' ? '23%' : '48%',
    marginBottom: 16,
  },
  tileLabel: {
    marginTop: 8,
    fontSize: 14,
    fontWeight: '500',
    textAlign: 'center',
  },
  interactiveSection: {
    flexDirection: 'column',
    marginTop: 24,
  },
  interactiveRow: {
    flexDirection: Platform.OS === 'web' ? 'row' : 'column',
    alignItems: Platform.OS === 'web' ? 'flex-start' : 'stretch',
    justifyContent: 'space-between',
  },
  controlsContainer: {
    flex: 1,
    marginTop: Platform.OS === 'web' ? 0 : 16,
    marginLeft: Platform.OS === 'web' ? 16 : 0,
  },
  controlsTitle: {
    fontSize: 16,
    fontWeight: '600',
    marginBottom: 12,
  },
  switchRow: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    marginVertical: 8,
  },
  switchLabel: {
    flex: 1,
  },
  pickerContainer: {
    marginVertical: 8,
    borderWidth: 1,
    borderColor: '#ccc',
    borderRadius: 4,
  },
  stateDisplay: {
    marginTop: 16,
    padding: 12,
    backgroundColor: '#f5f5f5',
    borderRadius: 4,
  },
  stateTitle: {
    fontWeight: '500',
    marginBottom: 4,
  },
  code: {
    fontFamily: Platform.OS === 'ios' ? 'Menlo' : Platform.OS === 'android' ? 'monospace' : 'Courier New',
    fontSize: 12,
  },
  hint: {
    marginTop: 8,
    fontStyle: 'italic',
    fontSize: 12,
    color: '#666',
  },
  exportSection: {
    marginTop: 24,
    marginBottom: 32,
    borderTopWidth: 1,
    borderTopColor: '#eee',
    paddingTop: 16,
  },
  exportTitle: {
    fontSize: 16,
    fontWeight: '600',
    marginBottom: 12,
  },
  exportButtonsRow: {
    flexDirection: 'row',
    justifyContent: 'space-around',
    marginTop: 8,
  },
  exportButton: {
    backgroundColor: '#007bff',
    paddingVertical: 8,
    paddingHorizontal: 16,
    borderRadius: 4,
    minWidth: 120,
    alignItems: 'center',
  },
  exportButtonText: {
    color: 'white',
    fontWeight: '500',
  },
});

// Main component
const MinesweeperTileGenerator = () => {
  const [customColor, setCustomColor] = useState('#d8d8d8');
  const [shadowColor, setShadowColor] = useState('#555555');

  // Render a tile based on props
  const renderTile = (props: {
    pressed?: boolean;
    showMine?: boolean;
    showFlag?: boolean;
    noBorder?: boolean;
    digit?: number | null;
    width?: number;
    height?: number;
  }) => {
    const {
      pressed = false,
      showMine = false,
      showFlag = false,
      noBorder = false,
      digit = null,
      width = 100,
      height = 100,
    } = props;

    return (
      <Svg width={width} height={height} viewBox="0 0 100 100">
        {/* Animations would normally go in Defs with Style tags
           but we'll just use static styling for compatibility */}

        {/* Outer frame */}
        <Path d="M5,5 L95,5 L95,95 L5,95 Z" fill="#888888" />
        
        {/* Main tile face */}
        <Path d="M10,10 L90,10 L90,90 L10,90 Z" fill={customColor} />
        
        {/* Simple 1px border for no-border mode */}
        {noBorder && (
          <Rect x="5" y="5" width="90" height="90" fill="none" stroke={shadowColor} strokeWidth="1" />
        )}
        
        {/* Highlight edges (top-left) */}
        {!noBorder && (
          <>
            <Path 
              d="M5,5 L95,5 L90,10 L10,10 Z" 
              fill={pressed ? shadowColor : "#ffffff"} 
            />
            <Path 
              d="M5,5 L10,10 L10,90 L5,95 Z" 
              fill={pressed ? "#777777" : "#f0f0f0"} 
            />
          </>
        )}
        
        {/* Shadow edges (bottom-right) */}
        {!noBorder && (
          <>
            <Path 
              d="M10,90 L90,90 L95,95 L5,95 Z" 
              fill={pressed ? "#ffffff" : shadowColor} 
            />
            <Path 
              d="M90,10 L95,5 L95,95 L90,90 Z" 
              fill={pressed ? "#f0f0f0" : "#777777"} 
            />
          </>
        )}
        
        {/* Mine components */}
        {showMine && (
          <G>
            {/* Glow effect */}
            <Circle cx="50" cy="50" r="30" fill="#ff0000" opacity="0.2" />
            
            {/* Mine body */}
            <Circle cx="50" cy="50" r="15" fill="#333333" />
            
            {/* Mine spikes */}
            <Path d="M50,20 L50,80" stroke="#333333" strokeWidth="4" />
            <Path d="M20,50 L80,50" stroke="#333333" strokeWidth="4" />
            <Path d="M29.3,29.3 L70.7,70.7" stroke="#333333" strokeWidth="4" />
            <Path d="M29.3,70.7 L70.7,29.3" stroke="#333333" strokeWidth="4" />
            
            {/* Mine highlights */}
            <Circle cx="45" cy="45" r="4" fill="#ffffff" />
          </G>
        )}
        
        {/* Flag components */}
        {showFlag && (
          <G>
            <Rect x="45" y="25" width="5" height="50" fill="#333333" />
            <Path d="M50,25 L75,35 L50,45 Z" fill="#ff0000" />
          </G>
        )}
        
        {/* Digit components (1-8) */}
        {digit && (
          <SvgText
            x="50"
            y="52"
            fontSize="45"
            fontWeight="bold"
            textAnchor="middle"
            fill={
              digit === 1 ? "#0000ff" : // Blue
              digit === 2 ? "#008000" : // Green
              digit === 3 ? "#ff0000" : // Red
              digit === 4 ? "#000080" : // Dark Blue
              digit === 5 ? "#800000" : // Maroon
              digit === 6 ? "#008080" : // Teal
              digit === 7 ? "#000000" : // Black
              digit === 8 ? "#808080" : // Gray
              "#000000" // Default
            }
          >
            {digit}
          </SvgText>
        )}
      </Svg>
    );
  };

  // Color picker component based on platform
  const ColorPickerComponent = ({ color, onColorChange, label }: { color: string, onColorChange: (color: string) => void, label: string }) => {
    // Always use WebColorPicker since we've removed the iOS ColorPicker
    return (
      <View style={styles.colorPickerItem}>
        <Text style={styles.colorLabel}>{label}</Text>
        {Platform.OS === 'web' ? (
          <WebColorPicker 
            color={color}
            onColorChange={onColorChange}
          />
        ) : (
          // For non-web platforms, just show a colored box
          <View 
            style={{ 
              height: 50, 
              backgroundColor: color, 
              borderWidth: 1, 
              borderColor: '#ccc', 
              borderRadius: 4 
            }} 
          />
        )}
        <Text style={styles.colorValue}>{color}</Text>
      </View>
    );
  };

  return (
    <CopilotProvider>
      <SafeAreaView style={styles.container}>
        <ScrollView style={styles.scrollView}>
          <Text style={styles.title}>MinesweeperTileGenerator</Text>
          
          <View style={styles.colorPickerContainer}>
            <ColorPickerComponent
              label="Background Color:"
              color={customColor}
              onColorChange={setCustomColor}
            />
            
            <ColorPickerComponent
              label="Shadow/Border Color:"
              color={shadowColor}
              onColorChange={setShadowColor}
            />
          </View>
          
          <Text style={styles.subtitle}>Basic Tile States</Text>
          <View style={styles.tilesGrid}>
            <View style={styles.tileContainer}>
              {renderTile({ pressed: false })}
              <Text style={styles.tileLabel}>Normal Tile</Text>
            </View>
            
            <View style={styles.tileContainer}>
              {renderTile({ pressed: true })}
              <Text style={styles.tileLabel}>Pressed Tile</Text>
            </View>
            
            <View style={styles.tileContainer}>
              {renderTile({ showFlag: true })}
              <Text style={styles.tileLabel}>Flagged Tile</Text>
            </View>
            
            <View style={styles.tileContainer}>
              {renderTile({ pressed: true, showMine: true })}
              <Text style={styles.tileLabel}>Revealed Mine</Text>
            </View>
          </View>
          
          <Text style={styles.subtitle}>Number Tiles (1-8)</Text>
          <View style={styles.tilesGrid}>
            {[1, 2, 3, 4, 5, 6, 7, 8].map(num => (
              <View key={num} style={styles.tileContainer}>
                {renderTile({ pressed: true, digit: num })}
                <Text style={styles.tileLabel}>Number {num}</Text>
              </View>
            ))}
          </View>
          
          <Text style={styles.subtitle}>Flat Tiles (No Border)</Text>
          <View style={styles.tilesGrid}>
            <View style={styles.tileContainer}>
              {renderTile({ noBorder: true })}
              <Text style={styles.tileLabel}>Flat Tile</Text>
            </View>
            
            <View style={styles.tileContainer}>
              {renderTile({ pressed: true, noBorder: true })}
              <Text style={styles.tileLabel}>Pressed Flat</Text>
            </View>
            
            <View style={styles.tileContainer}>
              {renderTile({ pressed: true, digit: 3, noBorder: true })}
              <Text style={styles.tileLabel}>Number 3 Flat</Text>
            </View>
            
            <View style={styles.tileContainer}>
              {renderTile({ pressed: true, showMine: true, noBorder: true })}
              <Text style={styles.tileLabel}>Mine Flat</Text>
            </View>
          </View>
          
          <Text style={styles.subtitle}>Interactive Demo</Text>
          <InteractiveTile 
            backgroundColor={customColor} 
            shadowColor={shadowColor} 
          />
        </ScrollView>
      </SafeAreaView>
    </CopilotProvider>
  );
};

// Interactive component that lets you toggle states
const InteractiveTile = ({ backgroundColor, shadowColor }: { backgroundColor: string, shadowColor: string }) => {
  const [pressed, setPressed] = useState(false);
  const [showMine, setShowMine] = useState(false);
  const [showFlag, setShowFlag] = useState(false);
  const [noBorder, setNoBorder] = useState(false);
  const [digit, setDigit] = useState<string | null>(null);
  
  const viewShotRef = useRef<any>(null);
  
  // Export the current tile as PNG
  const exportAsPNG = async () => {
    try {
      if (viewShotRef.current) {
        const uri = await viewShotRef.current.capture();
        
        if (Platform.OS === 'web') {
          // For web: Create a download link
          const link = document.createElement('a');
          link.href = uri;
          link.download = 'minesweeper-tile.png';
          document.body.appendChild(link);
          link.click();
          document.body.removeChild(link);
        } else {
          // For native: Save to device or share
          await Sharing.shareAsync(uri, {
            mimeType: 'image/png',
            dialogTitle: 'Save Minesweeper Tile'
          });
        }
        
        Alert.alert('Success', 'Tile exported as PNG');
      }
    } catch (error: any) {
      Alert.alert('Error', 'Failed to export tile: ' + error.message);
    }
  };
  
  // Export the current tile as SVG
  const exportAsSVG = () => {
    try {
      // Generate SVG code based on current settings
      const svgCode = `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100" width="100" height="100">
  <style>
    .tile-face { fill: ${backgroundColor}; }
    .highlight-top { fill: ${pressed ? shadowColor : "#ffffff"}; }
    .highlight-left { fill: ${pressed ? "#777777" : "#f0f0f0"}; }
    .shadow-bottom { fill: ${pressed ? "#ffffff" : shadowColor}; }
    .shadow-right { fill: ${pressed ? "#f0f0f0" : "#777777"}; }
    .simple-border { stroke: ${shadowColor}; stroke-width: 1; }
    @keyframes pulse {
      0% { opacity: 0.3; }
      50% { opacity: 1; }
      100% { opacity: 0.3; }
    }
    .mine-glow { animation: pulse 2s infinite; }
  </style>

  <!-- Outer frame -->
  <path d="M5,5 L95,5 L95,95 L5,95 Z" fill="#888888" />
  
  <!-- Main tile face -->
  <path d="M10,10 L90,10 L90,90 L10,90 Z" fill="${backgroundColor}" />
  
  ${noBorder ? `<rect x="5" y="5" width="90" height="90" fill="none" stroke="${shadowColor}" stroke-width="1" />` : ''}
  
  ${!noBorder ? `<!-- Highlight edges (top-left) -->
  <path d="M5,5 L95,5 L90,10 L10,10 Z" fill="${pressed ? shadowColor : "#ffffff"}" />
  <path d="M5,5 L10,10 L10,90 L5,95 Z" fill="${pressed ? "#777777" : "#f0f0f0"}" />
  
  <!-- Shadow edges (bottom-right) -->
  <path d="M10,90 L90,90 L95,95 L5,95 Z" fill="${pressed ? "#ffffff" : shadowColor}" />
  <path d="M90,10 L95,5 L95,95 L90,90 Z" fill="${pressed ? "#f0f0f0" : "#777777"}" />` : ''}
  
  ${showMine ? `<!-- Mine components -->
  <g>
    <!-- Glow effect -->
    <circle cx="50" cy="50" r="30" fill="#ff0000" opacity="0.2" class="mine-glow" />
    
    <!-- Mine body -->
    <circle cx="50" cy="50" r="15" fill="#333333" />
    
    <!-- Mine spikes -->
    <path d="M50,20 L50,80" stroke="#333333" stroke-width="4" />
    <path d="M20,50 L80,50" stroke="#333333" stroke-width="4" />
    <path d="M29.3,29.3 L70.7,70.7" stroke="#333333" stroke-width="4" />
    <path d="M29.3,70.7 L70.7,29.3" stroke="#333333" stroke-width="4" />
    
    <!-- Mine highlights -->
    <circle cx="45" cy="45" r="4" fill="#ffffff" />
  </g>` : ''}
  
  ${showFlag ? `<!-- Flag components -->
  <g>
    <rect x="45" y="25" width="5" height="50" fill="#333333" />
    <path d="M50,25 L75,35 L50,45 Z" fill="#ff0000" />
  </g>` : ''}
  
  ${digit ? `<!-- Digit -->
  <text x="50" y="52" font-size="45" font-weight="bold" text-anchor="middle" fill="${
    digit === '1' ? "#0000ff" : // Blue
    digit === '2' ? "#008000" : // Green
    digit === '3' ? "#ff0000" : // Red
    digit === '4' ? "#000080" : // Dark Blue
    digit === '5' ? "#800000" : // Maroon
    digit === '6' ? "#008080" : // Teal
    digit === '7' ? "#000000" : // Black
    digit === '8' ? "#808080" : // Gray
    "#000000" // Default
  }">${digit}</text>` : ''}
</svg>`;
      
      if (Platform.OS === 'web') {
        // For web: Copy to clipboard and offer download
        navigator.clipboard.writeText(svgCode)
          .then(() => {
            alert('SVG code copied to clipboard!');
            
            // Also offer download
            const blob = new Blob([svgCode], { type: 'image/svg+xml' });
            const url = URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = 'minesweeper-tile.svg';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            URL.revokeObjectURL(url);
          })
          .catch((err: Error) => {
            alert('Failed to copy SVG: ' + err.message);
          });
      } else {
        // For native: Save to file and share
        const path = `${FileSystem.CachesDirectoryPath}/minesweeper-tile.svg`;
        FileSystem.writeFile(path, svgCode, 'utf8')
          .then(() => {
            return Sharing.shareAsync(path, {
              mimeType: 'image/svg+xml',
              dialogTitle: 'Save Minesweeper Tile SVG'
            });
          })
          .then(() => {
            Alert.alert('Success', 'SVG exported successfully');
          })
          .catch((error: Error) => {
            Alert.alert('Error', 'Failed to export SVG: ' + error.message);
          });
      }
    } catch (error: any) {
      Alert.alert('Error', 'Failed to export SVG: ' + error.message);
    }
  };
  
  // Create a renderable tile component
  const TileComponent = () => (
    <Svg width={150} height={150} viewBox="0 0 100 100">
      {/* Animations would normally go in Defs with Style */}

      {/* Outer frame */}
      <Path d="M5,5 L95,5 L95,95 L5,95 Z" fill="#888888" />
      
      {/* Main tile face */}
      <Path d="M10,10 L90,10 L90,90 L10,90 Z" fill={backgroundColor} />
      
      {/* Simple 1px border for no-border mode */}
      {noBorder && (
        <Rect x="5" y="5" width="90" height="90" fill="none" stroke={shadowColor} strokeWidth="1" />
      )}
      
      {/* Highlight edges (top-left) */}
      {!noBorder && (
        <>
          <Path 
            d="M5,5 L95,5 L90,10 L10,10 Z" 
            fill={pressed ? shadowColor : "#ffffff"} 
          />
          <Path 
            d="M5,5 L10,10 L10,90 L5,95 Z" 
            fill={pressed ? "#777777" : "#f0f0f0"} 
          />
        </>
      )}
      
      {/* Shadow edges (bottom-right) */}
      {!noBorder && (
        <>
          <Path 
            d="M10,90 L90,90 L95,95 L5,95 Z" 
            fill={pressed ? "#ffffff" : shadowColor} 
          />
          <Path 
            d="M90,10 L95,5 L95,95 L90,90 Z" 
            fill={pressed ? "#f0f0f0" : "#777777"} 
          />
        </>
      )}
      
      {/* Mine components */}
      {showMine && (
        <G>
          {/* Glow effect */}
          <Circle cx="50" cy="50" r="30" fill="#ff0000" opacity="0.2" />
          
          {/* Mine body */}
          <Circle cx="50" cy="50" r="15" fill="#333333" />
          
          {/* Mine spikes */}
          <Path d="M50,20 L50,80" stroke="#333333" strokeWidth="4" />
          <Path d="M20,50 L80,50" stroke="#333333" strokeWidth="4" />
          <Path d="M29.3,29.3 L70.7,70.7" stroke="#333333" strokeWidth="4" />
          <Path d="M29.3,70.7 L70.7,29.3" stroke="#333333" strokeWidth="4" />
          
          {/* Mine highlights */}
          <Circle cx="45" cy="45" r="4" fill="#ffffff" />
        </G>
      )}
      
      {/* Flag components */}
      {showFlag && (
        <G>
          <Rect x="45" y="25" width="5" height="50" fill="#333333" />
          <Path d="M50,25 L75,35 L50,45 Z" fill="#ff0000" />
        </G>
      )}
      
      {/* Digit components (1-8) */}
      {digit && (
        <SvgText
          x="50"
          y="52"
          fontSize="45"
          fontWeight="bold"
          textAnchor="middle"
          fill={
            digit === '1' ? "#0000ff" : // Blue
            digit === '2' ? "#008000" : // Green
            digit === '3' ? "#ff0000" : // Red
            digit === '4' ? "#000080" : // Dark Blue
            digit === '5' ? "#800000" : // Maroon
            digit === '6' ? "#008080" : // Teal
            digit === '7' ? "#000000" : // Black
            digit === '8' ? "#808080" : // Gray
            "#000000" // Default
          }
        >
          {digit}
        </SvgText>
      )}
    </Svg>
  );

  return (
    <View style={styles.interactiveSection}>
      <View style={styles.interactiveRow}>
        <CopilotStep
          text="Tap this tile to toggle its pressed state"
          order={1}
          name="interactive-tile"
        >
          <WalkthroughableView>
            {Platform.OS === 'web' ? (
              <TouchableOpacity onPress={() => setPressed(!pressed)}>
                <TileComponent />
              </TouchableOpacity>
            ) : (
              <ViewShot ref={viewShotRef} options={{ format: 'png', quality: 1 }}>
                <TouchableOpacity onPress={() => setPressed(!pressed)}>
                  <TileComponent />
                </TouchableOpacity>
              </ViewShot>
            )}
          </WalkthroughableView>
        </CopilotStep>
        
        <View style={styles.controlsContainer}>
          <Text style={styles.controlsTitle}>Toggle States:</Text>
          
          <View style={styles.switchRow}>
            <Text style={styles.switchLabel}>Pressed</Text>
            <Switch
              value={pressed}
              onValueChange={setPressed}
            />
          </View>
          
          <View style={styles.switchRow}>
            <Text style={styles.switchLabel}>Show Mine</Text>
            <Switch
              value={showMine}
              onValueChange={(value) => {
                setShowMine(value);
                if (value) {
                  setDigit(null);
                  setShowFlag(false);
                }
              }}
            />
          </View>
          
          <View style={styles.switchRow}>
            <Text style={styles.switchLabel}>Show Flag</Text>
            <Switch
              value={showFlag}
              onValueChange={(value) => {
                setShowFlag(value);
                if (value) {
                  setDigit(null);
                  setShowMine(false);
                }
              }}
            />
          </View>
          
          <View style={styles.switchRow}>
            <Text style={styles.switchLabel}>No Border (Flat)</Text>
            <Switch
              value={noBorder}
              onValueChange={setNoBorder}
            />
          </View>
          
          <View>
            <Text style={styles.switchLabel}>Digit (1-8):</Text>
            <View style={styles.pickerContainer}>
              <Picker
                selectedValue={digit}
                onValueChange={(value) => {
                  setDigit(value);
                  if (value) {
                    setShowMine(false);
                    setShowFlag(false);
                  }
                }}
              >
                <Picker.Item label="None" value={null} />
                <Picker.Item label="1 (Blue)" value="1" />
                <Picker.Item label="2 (Green)" value="2" />
                <Picker.Item label="3 (Red)" value="3" />
                <Picker.Item label="4 (Dark Blue)" value="4" />
                <Picker.Item label="5 (Maroon)" value="5" />
                <Picker.Item label="6 (Teal)" value="6" />
                <Picker.Item label="7 (Black)" value="7" />
                <Picker.Item label="8 (Gray)" value="8" />
              </Picker>
            </View>
          </View>
          
          <View style={styles.stateDisplay}>
            <Text style={styles.stateTitle}>Current State:</Text>
            <Text style={styles.code}>
              {`<svg class="mine-tile"${pressed ? ' pressed="true"' : ''}${showMine ? ' show-mine="true"' : ''}${showFlag ? ' show-flag="true"' : ''}${noBorder ? ' no-border="true"' : ''}${digit ? ` digit="${digit}"` : ''}>`}
            </Text>
          </View>
          
          <Text style={styles.hint}>
            Tap on the tile to toggle pressed state, or use the controls
          </Text>
        </View>
      </View>
      
      <View style={styles.exportSection}>
        <Text style={styles.exportTitle}>Export Current Tile</Text>
        <Text>Save the current tile configuration for use in your app</Text>
        
        <View style={styles.exportButtonsRow}>
          <TouchableOpacity style={styles.exportButton} onPress={exportAsPNG}>
            <Text style={styles.exportButtonText}>Export as PNG</Text>
          </TouchableOpacity>
          
          <TouchableOpacity style={styles.exportButton} onPress={exportAsSVG}>
            <Text style={styles.exportButtonText}>Export as SVG</Text>
          </TouchableOpacity>
        </View>
      </View>
    </View>
  );
};

export default MinesweeperTileGenerator;
