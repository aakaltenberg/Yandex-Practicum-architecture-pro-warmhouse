package gateclient

import (
    "bytes"
    "context"
    "encoding/json"
    "fmt"
    "net/http"
    "time"
)

type GateClient struct {
    baseURL string
    client  *http.Client
}

func NewGateClient(baseURL string) *GateClient {
    return &GateClient{
        baseURL: baseURL,
        client: &http.Client{Timeout: 10 * time.Second},
    }
}

type Gate struct {
    ID          int       `json:"id"`
    Name        string    `json:"name"`
    Location    string    `json:"location"`
    Status      string    `json:"status"`
    HouseId     string    `json:"houseId"`
    LastUpdated time.Time `json:"last_updated"`
    CreatedAt   time.Time `json:"created_at"`
    Description string    `json:"description"`
}

type GateCreate struct {
    Id          int    `json:"id"`
    Name        string `json:"name"`
    Location    string `json:"location"`
    HouseId     string `json:"houseId"`
    Description string `json:"description"`
}

type GateUpdate struct {
    Name        string `json:"name"`
    Location    string `json:"location"`
    HouseId     string `json:"houseId"`
    Description string `json:"description"`
}

func (c *GateClient) CreateGate(ctx context.Context, req GateCreate) (*Gate, error) {
    body, _ := json.Marshal(req)
    httpReq, _ := http.NewRequestWithContext(ctx, "POST", c.baseURL+"/api/v1/gates", bytes.NewReader(body))
    httpReq.Header.Set("Content-Type", "application/json")
    resp, err := c.client.Do(httpReq)
    if err != nil {
        return nil, err
    }
    defer resp.Body.Close()
    if resp.StatusCode != http.StatusCreated {
        return nil, fmt.Errorf("unexpected status: %d", resp.StatusCode)
    }
    var gate Gate
    if err := json.NewDecoder(resp.Body).Decode(&gate); err != nil {
        return nil, err
    }
    return &gate, nil
}

func (c *GateClient) UpdateGate(ctx context.Context, id int, req GateUpdate) (*Gate, error) {
    body, _ := json.Marshal(req)
    httpReq, _ := http.NewRequestWithContext(ctx, "PUT", fmt.Sprintf("%s/api/v1/gates/%d", c.baseURL, id), bytes.NewReader(body))
    httpReq.Header.Set("Content-Type", "application/json")
    resp, err := c.client.Do(httpReq)
    if err != nil {
        return nil, err
    }
    defer resp.Body.Close()
    if resp.StatusCode != http.StatusOK {
        return nil, fmt.Errorf("unexpected status: %d", resp.StatusCode)
    }
    var gate Gate
    if err := json.NewDecoder(resp.Body).Decode(&gate); err != nil {
        return nil, err
    }
    return &gate, nil
}

func (c *GateClient) DeleteGate(ctx context.Context, id int) error {
    httpReq, _ := http.NewRequestWithContext(ctx, "DELETE", fmt.Sprintf("%s/api/v1/gates/%d", c.baseURL, id), nil)
    resp, err := c.client.Do(httpReq)
    if err != nil {
        return err
    }
    defer resp.Body.Close()
    if resp.StatusCode != http.StatusNoContent {
        return fmt.Errorf("unexpected status: %d", resp.StatusCode)
    }
    return nil
}

func (c *GateClient) OpenGate(ctx context.Context, id int) error {
    httpReq, _ := http.NewRequestWithContext(ctx, "POST", fmt.Sprintf("%s/api/v1/gates/%d/open", c.baseURL, id), nil)
    resp, err := c.client.Do(httpReq)
    if err != nil {
        return err
    }
    defer resp.Body.Close()
    if resp.StatusCode != http.StatusOK {
        return fmt.Errorf("unexpected status: %d", resp.StatusCode)
    }
    return nil
}

func (c *GateClient) CloseGate(ctx context.Context, id int) error {
    httpReq, _ := http.NewRequestWithContext(ctx, "POST", fmt.Sprintf("%s/api/v1/gates/%d/close", c.baseURL, id), nil)
    resp, err := c.client.Do(httpReq)
    if err != nil {
        return err
    }
    defer resp.Body.Close()
    if resp.StatusCode != http.StatusOK {
        return fmt.Errorf("unexpected status: %d", resp.StatusCode)
    }
    return nil
}

// GetGate получает ворота по ID из Gate Service
func (c *GateClient) GetGate(ctx context.Context, id int) (*Gate, error) {
    httpReq, err := http.NewRequestWithContext(ctx, "GET", fmt.Sprintf("%s/api/v1/gates/%d", c.baseURL, id), nil)
    if err != nil {
        return nil, err
    }
    resp, err := c.client.Do(httpReq)
    if err != nil {
        return nil, err
    }
    defer resp.Body.Close()
    if resp.StatusCode != http.StatusOK {
        return nil, fmt.Errorf("unexpected status: %d", resp.StatusCode)
    }
    var gate Gate
    if err := json.NewDecoder(resp.Body).Decode(&gate); err != nil {
        return nil, err
    }
    return &gate, nil
}

// GetAllGates получает список всех ворот из Gate Service
func (c *GateClient) GetAllGates(ctx context.Context) ([]Gate, error) {
    httpReq, err := http.NewRequestWithContext(ctx, "GET", c.baseURL+"/api/v1/gates", nil)
    if err != nil {
        return nil, err
    }
    resp, err := c.client.Do(httpReq)
    if err != nil {
        return nil, err
    }
    defer resp.Body.Close()
    if resp.StatusCode != http.StatusOK {
        return nil, fmt.Errorf("unexpected status: %d", resp.StatusCode)
    }
    var gates []Gate
    if err := json.NewDecoder(resp.Body).Decode(&gates); err != nil {
        return nil, err
    }
    return gates, nil
}